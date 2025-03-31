using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoPograAvanzada.Models;

namespace ProyectoPograAvanzada.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // Listar Usuarios con sus Roles
        public ActionResult Index()
        {
            var users = _context.Users.ToList().Select(u => new UserRoleViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                RoleName = u.Roles.Any() ? _context.Roles.Find(u.Roles.First().RoleId).Name : "Sin Rol"
            });

            return View(users);
        }

        // Vista para Asignar un Rol
        public ActionResult AssignRole(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return HttpNotFound();

            var roles = _context.Roles.ToList();
            var viewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles
            };

            return View(viewModel);
        }

        // Acción para Asignar un Rol
        [HttpPost]
        public ActionResult AssignRole(UserRoleViewModel model)
        {
            var user = _context.Users.Find(model.UserId);
            if (user == null) return HttpNotFound();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var oldRoles = userManager.GetRoles(user.Id).ToList();
            userManager.RemoveFromRoles(user.Id, oldRoles.ToArray());
            userManager.AddToRole(user.Id, _context.Roles.Find(model.RoleId).Name);

            return RedirectToAction("Index");
        }

        // Eliminar un Rol de un Usuario
        public ActionResult RemoveRole(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return HttpNotFound();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var roles = userManager.GetRoles(user.Id);
            if (roles.Any())
            {
                userManager.RemoveFromRoles(user.Id, roles.ToArray());
            }

            return RedirectToAction("Index");
        }
    }
}
