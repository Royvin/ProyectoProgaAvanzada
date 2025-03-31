using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    public class PedidosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pedidos
        public ActionResult Index()
        {
            var pedidos = db.Pedidos.ToList();
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
            // Cargar los items del pedido
            var items = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
            ViewBag.Items = items;
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public ActionResult Edit(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Cargar los items del pedido
            var items = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
            ViewBag.Items = items;

            // Preparar listas para los dropdown
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Entregado", Text = "Entregado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection form)
        {
            // Buscamos el pedido original
            var pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Confirmamos si el estado cambio de pendiente a entregado
            string nuevoEstado = form["Estado"];
            bool cambioAEntregado = false;

            if (pedido.Estado != "Entregado" && nuevoEstado == "Entregado")
            {
                cambioAEntregado = true;
            }

            // Actualizamos el estado del pedido
            pedido.Estado = nuevoEstado;

            try
            {
                // Optenemos todos los items de este pedido
                var items = db.PedidoItems.Where(i => i.IdPedido == id).ToList();

                // Realizamos este foreach para recorrer todos los items y encontrar las cantidades actualizadas
                foreach (string key in form.AllKeys)
                {
                    if (key.StartsWith("itemCantidad[") && key.EndsWith("]"))
                    {
                        // Extraemos el ID desde la Key
                        string itemIdStr = key.Substring(13, key.Length - 14);
                        int itemId;

                        if (int.TryParse(itemIdStr, out itemId))
                        {
                            // Encontramos el Item que coincida
                            var item = items.FirstOrDefault(i => i.Id == itemId);
                            if (item != null)
                            {
                                // Actualizamos las cantidades
                                int nuevaCantidad;
                                if (int.TryParse(form[key], out nuevaCantidad) && nuevaCantidad > 0)
                                {
                                    item.Cantidad = nuevaCantidad;
                                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                        }
                    }
                }

                // Si el estado cambio a entregado actualizar el inventario
                if (cambioAEntregado)
                {
                    foreach (var item in items)
                    {
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
                    TempData["Success"] = "Pedido actualizado y marcado como entregado. Se ha actualizado el inventario.";
                }
                else
                {
                    TempData["Success"] = "Pedido actualizado correctamente.";
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);

                // Recaragarmos los datos para la vista
                var itemsParaVista = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
                ViewBag.Items = itemsParaVista;
                ViewBag.Estados = new List<SelectListItem>
        {
            new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
            new SelectListItem { Value = "Entregado", Text = "Entregado" },
            new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
        };

                return View(pedido);
            }
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

            // Crear un nuevo pedido
            var pedido = new Pedidos
            {
                idCarrito = carritoId,
                FechaCompra = DateTime.Now,
                Estado = "Pendiente"
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();

            // Transferir items del carrito al pedido
            foreach (var item in carrito.Items)
            {
                var pedidoItem = new PedidoItem
                {
                    IdPedido = pedido.IdPedido,
                    ProductoId = item.ProductoId,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad
                };
                db.PedidoItems.Add(pedidoItem);
            }

            // Guardar los cambios
            db.SaveChanges();

            // Limpiar el carrito
            db.CarritoItems.RemoveRange(carrito.Items);
            db.SaveChanges();

            TempData["Success"] = "Pedido creado correctamente con el número: " + pedido.IdPedido;
            return RedirectToAction("Details", new { id = pedido.IdPedido });
        }

        // POST: Pedidos/ActualizarEstado
        [HttpPost]
        public ActionResult ActualizarEstado(int pedidoId, string nuevoEstado)
        {
            var pedido = db.Pedidos.Find(pedidoId);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Si el pedido ya estaba en estado "Entregado", no hacemos nada
            if (pedido.Estado == "Entregado" && nuevoEstado == "Entregado")
            {
                return RedirectToAction("Index");
            }

            // Actualizar el estado del pedido
            pedido.Estado = nuevoEstado;

            // Si el nuevo estado es "Entregado", reducir el inventario
            if (nuevoEstado == "Entregado")
            {
                // Obtener todos los elementos del pedido
                var pedidoItems = db.PedidoItems.Where(i => i.IdPedido == pedidoId).ToList();

                foreach (var item in pedidoItems)
                {
                    // Buscar el producto correspondiente
                    var producto = db.Productos.Find(item.ProductoId);
                    if (producto != null)
                    {
                        // Reducir la cantidad disponible
                        producto.CantidadaDisponible -= item.Cantidad;
                        // Asegurar que el stock no sea negativo
                        if (producto.CantidadaDisponible < 0)
                        {
                            producto.CantidadaDisponible = 0;
                        }
                    }
                }

                TempData["Success"] = "Pedido marcado como entregado. Se ha actualizado el inventario.";
            }
            else
            {
                TempData["Success"] = "Estado del pedido actualizado correctamente.";
            }

            db.SaveChanges();
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