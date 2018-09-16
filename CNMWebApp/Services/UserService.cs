using CNMWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace CNMWebApp.Services
{
    public class UserService
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        

        public UserService()
        {
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        }

        public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _userManager.Users;

            return users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            });
        }

        public async Task Create(UserRolesViewModel user)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(user.SelectedRoleId);

                var result = await _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName
                }, "Test123.");

                if (result.Succeeded)
                {
                    var userSaved = await _userManager.FindByNameAsync(user.UserName);
                    await _userManager.AddToRoleAsync(userSaved.Id.ToString(), role.Name);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}
    }
}