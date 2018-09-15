using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace CNMWebApp.Services
{
    public class UserService
    {
        private ApplicationUserManager _userManager;

        public UserService()
        {
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return _userManager.Users;
        }
    }
}