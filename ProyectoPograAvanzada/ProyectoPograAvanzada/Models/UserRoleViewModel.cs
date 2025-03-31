using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProyectoPograAvanzada.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Rol")]
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
