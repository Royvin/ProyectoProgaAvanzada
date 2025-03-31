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
                new SelectListItem { Value = "Entregado", Text = "Entregado" }
            };

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection form)
        {
            // Encontrar el pedido original
            var pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Verificar si el estado cambió de Pendiente a Entregado
            string nuevoEstado = form["Estado"];
            bool cambioAEntregado = false;

            if (pedido.Estado != "Entregado" && nuevoEstado == "Entregado")
            {
                cambioAEntregado = true;
            }

            try
            {
                // Obtener todos los items para este pedido
                var items = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
                var itemsActualizados = new List<PedidoItem>();

                // Primera pasada: Analizar y validar todas las cantidades sin actualizar la base de datos
                foreach (string key in form.AllKeys)
                {
                    if (key.StartsWith("itemCantidad[") && key.EndsWith("]"))
                    {
                        // Extraer el ID del item de la clave (formato: itemCantidad[123])
                        string itemIdStr = key.Substring(13, key.Length - 14);
                        int itemId;

                        if (int.TryParse(itemIdStr, out itemId))
                        {
                            // Encontrar el item correspondiente
                            var item = items.FirstOrDefault(i => i.Id == itemId);
                            if (item != null)
                            {
                                // Analizar la cantidad
                                int nuevaCantidad;
                                if (int.TryParse(form[key], out nuevaCantidad) && nuevaCantidad > 0)
                                {
                                    // Si cambia a Entregado, verificar disponibilidad de inventario
                                    if (cambioAEntregado)
                                    {
                                        var producto = db.Productos.Find(item.ProductoId);
                                        if (producto != null && producto.CantidadaDisponible < nuevaCantidad)
                                        {
                                            // Inventario insuficiente
                                            ModelState.AddModelError("", $"No hay suficiente inventario para el producto '{item.Nombre}'. Disponible: {producto.CantidadaDisponible}, Solicitado: {nuevaCantidad}");
                                            continue;
                                        }
                                    }

                                    // Almacenar para la segunda pasada
                                    item.Cantidad = nuevaCantidad;
                                    itemsActualizados.Add(item);
                                }
                            }
                        }
                    }
                }

                // Si hay errores de validación, volver a la vista
                if (!ModelState.IsValid)
                {
                    // Recargar datos para la vista
                    ViewBag.Items = items;
                    ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Entregado", Text = "Entregado" }
            };

                    return View(pedido);
                }

                // Actualizar el estado del pedido
                pedido.Estado = nuevoEstado;
                db.Entry(pedido).State = System.Data.Entity.EntityState.Modified;

                // Segunda pasada: Actualizar cantidades en la base de datos
                foreach (var item in itemsActualizados)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                // Si cambió a Entregado, actualizar el inventario
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

                // Recargar datos para la vista
                var itemsParaVista = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
                ViewBag.Items = itemsParaVista;
                ViewBag.Estados = new List<SelectListItem>
        {
            new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
            new SelectListItem { Value = "Entregado", Text = "Entregado" }
        };

                return View(pedido);
            }
        }

        // GET: Pedidos/Delete/5
        public ActionResult Delete(int id)
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

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Verificar si el pedido ya está entregado
            if (pedido.Estado == "Entregado")
            {
                TempData["Error"] = "No se puede eliminar un pedido que ya ha sido entregado, ya que afectaría al inventario.";
                return RedirectToAction("Index");
            }

            // Eliminar primero los items del pedido
            var pedidoItems = db.PedidoItems.Where(i => i.IdPedido == id).ToList();
            db.PedidoItems.RemoveRange(pedidoItems);

            // Luego eliminar el pedido
            db.Pedidos.Remove(pedido);
            db.SaveChanges();

            TempData["Success"] = "Pedido eliminado correctamente.";
            return RedirectToAction("Index");
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