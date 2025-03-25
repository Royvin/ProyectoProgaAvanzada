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
    public class DetallePedidosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DetallePedidos
        public ActionResult Index()
        {
            return View(db.DetallePedidos.ToList());
        }

        // GET: DetallePedidos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePedidos detallePedidos = db.DetallePedidos.Find(id);
            if (detallePedidos == null)
            {
                return HttpNotFound();
            }
            return View(detallePedidos);
        }

        // GET: DetallePedidos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetallePedidos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdDetalle,IdPedido,IdItem")] DetallePedidos detallePedidos)
        {
            if (ModelState.IsValid)
            {
                db.DetallePedidos.Add(detallePedidos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(detallePedidos);
        }

        // GET: DetallePedidos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePedidos detallePedidos = db.DetallePedidos.Find(id);
            if (detallePedidos == null)
            {
                return HttpNotFound();
            }
            return View(detallePedidos);
        }

        // POST: DetallePedidos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdDetalle,IdPedido,IdItem")] DetallePedidos detallePedidos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detallePedidos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(detallePedidos);
        }

        // GET: DetallePedidos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePedidos detallePedidos = db.DetallePedidos.Find(id);
            if (detallePedidos == null)
            {
                return HttpNotFound();
            }
            return View(detallePedidos);
        }

        // POST: DetallePedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetallePedidos detallePedidos = db.DetallePedidos.Find(id);
            db.DetallePedidos.Remove(detallePedidos);
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
