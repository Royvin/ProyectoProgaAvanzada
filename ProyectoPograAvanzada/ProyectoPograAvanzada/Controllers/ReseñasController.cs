using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    public class ReseñasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reseñas
        public ActionResult Index()
        {
            return View(db.Reseñas.ToList());
        }

        // GET: Reseñas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reseñas reseñas = db.Reseñas.Find(id);
            if (reseñas == null)
            {
                return HttpNotFound();
            }
            return View(reseñas);
        }

        // GET: Reseñas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reseñas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdReseña,Titulo,Descripcion")] Reseñas reseñas)
        {
            if (ModelState.IsValid)
            {
                db.Reseñas.Add(reseñas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reseñas);
        }

        // GET: Reseñas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reseñas reseñas = db.Reseñas.Find(id);
            if (reseñas == null)
            {
                return HttpNotFound();
            }
            return View(reseñas);
        }

        // POST: Reseñas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdReseña,Titulo,Descripcion")] Reseñas reseñas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reseñas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reseñas);
        }

        // GET: Reseñas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reseñas reseñas = db.Reseñas.Find(id);
            if (reseñas == null)
            {
                return HttpNotFound();
            }
            return View(reseñas);
        }

        // POST: Reseñas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reseñas reseñas = db.Reseñas.Find(id);
            db.Reseñas.Remove(reseñas);
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
