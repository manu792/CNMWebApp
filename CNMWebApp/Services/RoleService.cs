using CNMWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class RoleService
    {
        private ApplicationRoleManager _roleManager;

        public RoleService()
        {
            _roleManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _roleManager.Roles;
        }

        public IdentityRole ObtenerRolPorId(string rolId)
        {
            return _roleManager.Roles.FirstOrDefault(x => x.Id == rolId);
        }

        public IdentityRole ObtenerRolPorNombre(string rolNombre)
        {
            return _roleManager.Roles.FirstOrDefault(x => x.Name.Equals(rolNombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}