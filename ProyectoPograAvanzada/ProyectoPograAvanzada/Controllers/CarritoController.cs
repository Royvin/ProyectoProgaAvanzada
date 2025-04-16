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
            // Obtener o crear un carrito para el usuario actual
            var carrito = BuscarOCrearCarrito();
            // Buscar si el producto ya está en el carrito
            var carritoItem = db.CarritoItems.FirstOrDefault(p => p.ProductoId == id && p.Carrito.Id == carrito.Id);
            if (carritoItem != null)
            {
                // Si ya existe, incrementar cantidad
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
            return RedirectToAction("Index", "Productoes");
        }

        public ActionResult EliminarDeCarrito(int id)
        {
            var item = db.CarritoItems.FirstOrDefault(p => p.ProductoId == id);
            if (item != null)
            {
                db.CarritoItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Nuevo método para proceder con el pedido
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

            // Buscar si ya existe un carrito para este usuario
            var carrito = db.Carritos.FirstOrDefault(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                // Si no existe, crear uno nuevo asociado a este usuario
                carrito = new Carrito
                {
                    UsuarioId = usuarioId
                };
                db.Carritos.Add(carrito);
                db.SaveChanges();
            }

            return carrito;
        }
    }
}