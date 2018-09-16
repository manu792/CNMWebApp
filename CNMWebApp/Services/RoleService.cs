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

        public UserRolesViewModel GetAllRoles()
        {
            var roles = _roleManager.Roles;

            return new UserRolesViewModel()
            {
                Roles = roles.ToList()
            };
        }
    }
}