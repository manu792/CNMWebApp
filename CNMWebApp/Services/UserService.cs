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
            var users = _userManager.Users
                .Where(x => !x.Email.Equals("manager@manager.com"))
                .ToList();

            return users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Nombre = x.Nombre,
                PrimerApellido = x.PrimerApellido,
                SegundoApellido = x.SegundoApellido,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                FechaIngreso = x.FechaIngreso,
                Role = _roleManager.FindById(x.Roles.First().RoleId.ToString()),
                UnidadTecnica = x.UnidadTecnica,
                Categoria = x.Categoria
            }).ToList();
        }

        public IEnumerable<UserViewModel> ObtenerJefes()
        {
            var managerRol = _roleManager.FindByName("Manager");
            var jefaturaRol = _roleManager.FindByName("Jefatura");

            var users = _userManager.Users.Where(x => x.Roles.Any(r => r.RoleId == managerRol.Id || r.RoleId == jefaturaRol.Id));

            return users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Nombre = x.Nombre,
                PrimerApellido = x.PrimerApellido,
                SegundoApellido = x.SegundoApellido,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                FechaIngreso = x.FechaIngreso,
                Role = _roleManager.FindById(x.Roles.First().RoleId),
                UnidadTecnica = x.UnidadTecnica,
                Categoria = x.Categoria
            });
        }

        public async Task<ApplicationUser> GetLoggedInUser()
        {
            return await _userManager.FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());
        }

        public async Task<bool> Create(UserRolesUnidadCategoria user)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(user.SelectedRoleId);

                var result = await _userManager.CreateAsync(new ApplicationUser()
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    PrimerApellido = user.PrimerApellido,
                    SegundoApellido = user.SegundoApellido,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.Email,
                    FechaIngreso = user.FechaIngreso,
                    UnidadTecnicaId = Convert.ToInt32(user.SelectedUnidadTecnicaId),
                    CategoriaId = Convert.ToInt32(user.SelectedCategoriaId)
                }, "Test123.");

                if (result.Succeeded)
                {
                    var userSaved = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(userSaved.Id.ToString(), role.Name);

                    return true;
                }

                return false;
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