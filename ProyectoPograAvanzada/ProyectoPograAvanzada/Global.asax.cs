using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using ProyectoPograAvanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProyectoPograAvanzada
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //CrearRolesyUsuarios();
        }
        private void CrearRolesyUsuarios()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);

                var user = new ApplicationUser
                {
                    UserName = "admin@ufide.ac.cr",
                    Email = "admin@ufide.ac.cr"
                };

                var password = "Admin123";
                var result = userManager.Create(user, password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }

            }

            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole("User");
                roleManager.Create(role);
            }

        }
    }


}
