using CNMWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Data.Entity;

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
                Categoria = x.Categoria,
                EstaActivo = x.EstaActivo
            }).ToList();
        }

        public UserViewModel ObtenerUsuarioPorId(string id)
        {
            var user = _userManager.FindById(id);
            if (user == null)
                return new UserViewModel();

            return new UserViewModel()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FechaIngreso = user.FechaIngreso,
                Role = _roleManager.FindById(user.Roles.First().RoleId.ToString()),
                UnidadTecnica = user.UnidadTecnica,
                Categoria = user.Categoria,
                EstaActivo = user.EstaActivo,
                FotoRuta = user.FotoRuta
            };
        }

        public bool ActualizarUsuario(string id, UserRolesUnidadCategoria usuario)
        {
            try
            {
                var user = _userManager.FindById(id);
                if (user == null)
                    return false;

                // Reemplazo el rol del usuario en caso que se haya seleccionado uno diferente
                var roleId = user.Roles.FirstOrDefault().RoleId;
                var role = _roleManager.Roles.FirstOrDefault(r => r.Id == roleId);
                var newRole = _roleManager.Roles.FirstOrDefault(r => r.Id == usuario.SelectedRoleId);

                if (newRole.Id != role.Id)
                {
                    _userManager.RemoveFromRole(user.Id, role.Name);
                    _userManager.AddToRole(user.Id, newRole.Name);
                }

                // Actualizo usuario con nuevos datos
                user.Id = usuario.Id;
                user.Nombre = usuario.Nombre;
                user.PrimerApellido = usuario.PrimerApellido;
                user.SegundoApellido = usuario.SegundoApellido;
                user.Email = usuario.Email;
                user.UserName = usuario.Email;
                user.PhoneNumber = usuario.PhoneNumber;
                user.FechaIngreso = usuario.FechaIngreso;
                user.UnidadTecnicaId = Convert.ToInt32(usuario.SelectedUnidadTecnicaId);
                user.CategoriaId = Convert.ToInt32(usuario.SelectedCategoriaId);
                user.EstaActivo = usuario.EstaActivo;
                user.FotoRuta = usuario.FotoRuta;

                var resultado = _userManager.Update(user);

                if (resultado.Succeeded)
                    return true;

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool EliminarUsuario(string id)
        {
            try
            {
                var usuario = _userManager.FindById(id);
                var resultado = _userManager.Delete(usuario);
                if (resultado.Succeeded)
                    return true;

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        //public IEnumerable<UserViewModel> ObtenerJefes()
        //{
        //    var managerRol = _roleManager.FindByName("Manager");
        //    var jefaturaRol = _roleManager.FindByName("Jefatura");

        //    var users = _userManager.Users.Where(x => x.Roles.Any(r => r.RoleId == managerRol.Id || r.RoleId == jefaturaRol.Id));

        //    return users.Select(x => new UserViewModel()
        //    {
        //        Id = x.Id,
        //        Nombre = x.Nombre,
        //        PrimerApellido = x.PrimerApellido,
        //        SegundoApellido = x.SegundoApellido,
        //        Email = x.Email,
        //        PhoneNumber = x.PhoneNumber,
        //        FechaIngreso = x.FechaIngreso,
        //        Role = _roleManager.FindById(x.Roles.First().RoleId),
        //        UnidadTecnica = x.UnidadTecnica,
        //        Categoria = x.Categoria
        //    });
        //}

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
                    CategoriaId = Convert.ToInt32(user.SelectedCategoriaId),
                    EstaActivo = true,
                    FotoRuta = user.Foto != null ? Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"), user.Foto.FileName) : null
                }, "Test123.");

                if (result.Succeeded)
                {
                    var userSaved = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(userSaved.Id.ToString(), role.Name);

                    // Guardar foto de usuario, en caso que se haya seleccionado alguna
                    GuardarFoto(user.Foto);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void GuardarFoto(HttpPostedFileWrapper foto)
        {
            try
            {
                if(foto != null)
                {
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Fotos")))
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"));

                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"), foto.FileName);
                    foto.SaveAs(path);
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