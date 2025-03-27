using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    [Authorize] // Solo admin puede gestionar usuarios
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager;

        public UserController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: User
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // GET: User/Edit/id
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                if (user != null)
                {
                    user.NombreCompleto = model.NombreCompleto;
                    user.Carrera = model.Carrera;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: User/Delete/id
        public ActionResult Delete(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
