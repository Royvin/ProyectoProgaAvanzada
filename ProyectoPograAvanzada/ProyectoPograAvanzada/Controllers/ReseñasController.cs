using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    public class ReseñasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reseñas/Create/{idProducto}
        public ActionResult Create(int? idProducto)
        {
            if (idProducto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.IdProducto = idProducto;
            return View();
        }

        // POST: Reseñas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdReseña,Titulo,Descripcion,Calificacion,IdProducto")] Reseñas reseña)
        {
            if (ModelState.IsValid)
            {
                reseña.FechaCreacion = DateTime.Now;
                db.Reseñas.Add(reseña);
                db.SaveChanges();
                return RedirectToAction("Index", "Productoes");
            }
            return View(reseña);
        }

        public ActionResult Delete(int id)
        {
            var reseña = db.Reseñas.Find(id);
            if (reseña == null)
            {
                return HttpNotFound();
            }
            return View(reseña);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var reseña = db.Reseñas.Find(id);
            if (reseña != null)
            {
                db.Reseñas.Remove(reseña);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Productoes", new { id = reseña.IdProducto });
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
