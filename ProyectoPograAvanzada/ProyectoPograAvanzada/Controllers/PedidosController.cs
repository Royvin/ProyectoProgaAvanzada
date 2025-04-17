using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pedidos
        public ActionResult Index()
        {
            string usuarioId = User.Identity.GetUserId();
            var pedidos = db.Pedidos.Where(p => p.UsuarioId == usuarioId).ToList();

            return View(pedidos);
        }

        // GET: Pedidos/Details/5
        public ActionResult Details(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Verificar que el pedido pertenece al usuario actual o es admin
            string usuarioId = User.Identity.GetUserId();
            if (pedido.UsuarioId != usuarioId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Cargar los items del pedido
            var items = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
            ViewBag.Items = items;
            return View(pedido);
        }

        // Este método será llamado desde el CarritoController
        public ActionResult CrearPedido(int carritoId)
        {
            var carrito = db.Carritos.Find(carritoId);
            if (carrito == null)
            {
                return HttpNotFound();
            }

            // Cargar los items del carrito
            db.Entry(carrito).Collection(c => c.Items).Load();

            // Verificar que el carrito tenga items
            if (carrito.Items.Count == 0)
            {
                TempData["Error"] = "No hay productos en el carrito";
                return RedirectToAction("Index", "Carrito");
            }

            // Verificar disponibilidad de inventario antes de crear el pedido
            foreach (var item in carrito.Items)
            {
                var producto = db.Productos.Find(item.ProductoId);
                if (producto == null)
                {
                    TempData["Error"] = $"Producto no encontrado: {item.Nombre}";
                    return RedirectToAction("Index", "Carrito");
                }

                if (producto.CantidadaDisponible < item.Cantidad)
                {
                    TempData["Error"] = $"No hay suficiente inventario para: {item.Nombre}. Disponible: {producto.CantidadaDisponible}";
                    return RedirectToAction("Index", "Carrito");
                }
            }

            // Obtener el ID del usuario actual
            string usuarioId = User.Identity.GetUserId();

            // Crear un nuevo pedido con el ID del usuario
            var pedido = new Pedidos
            {
                idCarrito = carritoId,
                FechaCompra = DateTime.Now,
                Estado = "Pendiente",
                UsuarioId = usuarioId // Asignamos el ID del usuario actual
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();

            // Transferir items del carrito al pedido y actualizar inventario
            foreach (var item in carrito.Items)
            {
                // Crear el item del pedido
                var pedidoItem = new PedidoItem
                {
                    IdPedido = pedido.IdPedido,
                    ProductoId = item.ProductoId,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad
                };
                db.PedidoItems.Add(pedidoItem);

                // Actualizar el inventario inmediatamente
                var producto = db.Productos.Find(item.ProductoId);
                if (producto != null)
                {
                    producto.CantidadaDisponible -= item.Cantidad;
                    if (producto.CantidadaDisponible < 0)
                    {
                        producto.CantidadaDisponible = 0;
                    }
                    db.Entry(producto).State = System.Data.Entity.EntityState.Modified;
                }
            }

            // Guardar los cambios
            db.SaveChanges();

            // Limpiar el carrito
            db.CarritoItems.RemoveRange(carrito.Items);
            db.SaveChanges();

            TempData["Success"] = "Pedido creado correctamente con el número: " + pedido.IdPedido + ". El inventario ha sido actualizado.";
            return RedirectToAction("Details", new { id = pedido.IdPedido });
        }

        // POST: Pedidos/ActualizarEstado
        [HttpPost]
        public ActionResult ActualizarEstado(int pedidoId, string nuevoEstado)
        {
            var pedido = db.Pedidos.Find(pedidoId);

            if (pedido == null)
            {
                TempData["Error"] = "Pedido no encontrado.";
                return RedirectToAction("Index");
            }

            // Verificar si el pedido ya está marcado como entregado
            if (pedido.Estado == "Entregado")
            {
                TempData["Error"] = "No se puede modificar un pedido que ya ha sido entregado.";
                return RedirectToAction("Index");
            }

            // Actualizar el estado del pedido solo si es válido
            if (nuevoEstado == "Pendiente" || nuevoEstado == "Entregado")
            {
                pedido.Estado = nuevoEstado;
                db.SaveChanges();

                TempData["Success"] = $"Estado del pedido #{pedidoId} actualizado a '{nuevoEstado}' correctamente.";
            }
            else
            {
                TempData["Error"] = "Estado no válido.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}