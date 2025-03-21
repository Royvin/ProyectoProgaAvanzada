using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
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

        public ActionResult Checkout()
        {
            // Aquí iría la lógica para procesar la compra y limpiar el carrito
            return View();
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
            // Aquí puedes implementar lógica para identificar al usuario
            // Por ahora, usaremos un solo carrito genérico

            // Buscar si ya existe un carrito
            var carrito = db.Carritos.FirstOrDefault();

            if (carrito == null)
            {
                // Si no existe, crear uno nuevo
                carrito = new Carrito();
                db.Carritos.Add(carrito);
                db.SaveChanges();
            }

            return carrito;
        }
    }
}