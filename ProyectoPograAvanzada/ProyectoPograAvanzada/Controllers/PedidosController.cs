using System;
using System.Collections.Generic;
using System.Linq;
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