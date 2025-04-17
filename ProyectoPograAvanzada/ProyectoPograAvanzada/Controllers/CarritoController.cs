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
    public class CarritoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var carrito = BuscarOCrearCarrito();
            // Cargar explícitamente los items si es necesario
            db.Entry(carrito).Collection(c => c.Items).Load();
            return View(carrito.Items.ToList());
        }

        public ActionResult AnadirCarrito(int id)
        {
            var producto = db.Productos.Find(id);
            if (producto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Verificar stock disponible
            if (producto.CantidadaDisponible <= 0)
            {
                TempData["Error"] = $"No hay stock disponible para el producto: {producto.Nombre}";
                return RedirectToAction("Index", "Home");
            }

            // Obtener o crear un carrito para el usuario actual
            var carrito = BuscarOCrearCarrito();

            // Buscar si el producto ya está en el carrito
            var carritoItem = db.CarritoItems.FirstOrDefault(p => p.ProductoId == id && p.Carrito.Id == carrito.Id);
            if (carritoItem != null)
            {
                // Si ya existe, incrementar cantidad solo si hay stock suficiente
                if (carritoItem.Cantidad + 1 > producto.CantidadaDisponible)
                {
                    TempData["Error"] = $"No hay suficiente stock para {producto.Nombre}. Stock disponible: {producto.CantidadaDisponible}";
                    return RedirectToAction("Index", "Home");
                }

                carritoItem.Cantidad++;
            }
            else
            {
                // Si no existe, crear nuevo item
                var nuevoItem = new CarritoItem
                {
                    ProductoId = id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = 1
                };
                // Asociar el item al carrito
                carrito.Items.Add(nuevoItem);
            }

            db.SaveChanges();

            // Añadir mensaje de confirmación
            TempData["Message"] = $"¡{producto.Nombre} ha sido añadido al carrito!";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EliminarDeCarrito(int id)
        {
            var carrito = BuscarOCrearCarrito();
            var item = db.CarritoItems.FirstOrDefault(p => p.ProductoId == id && p.Carrito.Id == carrito.Id);
            if (item != null)
            {
                db.CarritoItems.Remove(item);
                db.SaveChanges();
                TempData["Success"] = "Producto eliminado del carrito.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ActualizarCantidad(int productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                TempData["Error"] = "La cantidad debe ser mayor a 0";
                return RedirectToAction("Index");
            }

            var carrito = BuscarOCrearCarrito();
            var carritoItem = db.CarritoItems.FirstOrDefault(p => p.ProductoId == productoId && p.Carrito.Id == carrito.Id);

            if (carritoItem == null)
            {
                TempData["Error"] = "Producto no encontrado en el carrito";
                return RedirectToAction("Index");
            }

            // Verificar stock disponible
            var producto = db.Productos.Find(productoId);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Index");
            }

            if (cantidad > producto.CantidadaDisponible)
            {
                TempData["Error"] = $"No hay suficiente stock para {producto.Nombre}. Stock disponible: {producto.CantidadaDisponible}";
                return RedirectToAction("Index");
            }

            // Actualizar cantidad
            carritoItem.Cantidad = cantidad;
            db.SaveChanges();

            TempData["Success"] = "Cantidad actualizada correctamente";
            return RedirectToAction("Index");
        }

        // Método para proceder con el pedido
        public ActionResult Pedido()
        {
            var carrito = BuscarOCrearCarrito();

            // Verificar que el carrito tenga items
            db.Entry(carrito).Collection(c => c.Items).Load();
            if (carrito.Items.Count == 0)
            {
                TempData["Error"] = "No hay productos en el carrito";
                return RedirectToAction("Index");
            }

            // Verificar stock disponible para cada producto antes de proceder
            bool hayErrorInventario = false;

            foreach (var item in carrito.Items)
            {
                var producto = db.Productos.Find(item.ProductoId);
                if (producto == null)
                {
                    TempData["Error"] = $"Producto no encontrado: {item.Nombre}";
                    hayErrorInventario = true;
                    break;
                }

                if (producto.CantidadaDisponible < item.Cantidad)
                {
                    TempData["Error"] = $"No hay suficiente inventario para: {item.Nombre}. Disponible: {producto.CantidadaDisponible}";
                    hayErrorInventario = true;
                    break;
                }
            }

            if (hayErrorInventario)
            {
                return RedirectToAction("Index");
            }

            // Redirigir al método CrearPedido del PedidosController
            return RedirectToAction("CrearPedido", "Pedidos", new { carritoId = carrito.Id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Carrito BuscarOCrearCarrito()
        {
            // Obtener el ID del usuario actual
            string usuarioId = User.Identity.GetUserId();

            // Si el usuario no está autenticado, usar ID de sesión
            if (string.IsNullOrEmpty(usuarioId))
            {
                usuarioId = Session.SessionID;
            }

            // Buscar si ya existe un carrito para este usuario
            var carrito = db.Carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                // Si no existe, crear uno nuevo asociado a este usuario
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    Items = new List<CarritoItem>()
                };
                db.Carritos.Add(carrito);
                db.SaveChanges();
            }

            return carrito;
        }
    }
}