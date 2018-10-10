using CNMWebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Web.Security;
using System.Data.Entity;

namespace CNMWebApp.Services
{
    public class UserService
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private CategoriaServicio _categoriaServicio;
        

        public UserService()
        {
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            _categoriaServicio = new CategoriaServicio();
        }

        public UserViewModel ObtenerJefe(int unidadTecnicaId)
        {
            var jefaturaRole = _roleManager.FindByName("Jefatura");
            var jefe = _userManager.Users
                .Where(x => x.UnidadTecnicaId == unidadTecnicaId && x.Roles.Any(r => r.RoleId == jefaturaRole.Id))
                .FirstOrDefault();

            if (jefe == null)
                throw new Exception("No se encontró ningún jefe para la unidad técnica seleccionada");

            return new UserViewModel()
            {
                Id = jefe.Id,
                Nombre = jefe.Nombre,
                PrimerApellido = jefe.PrimerApellido,
                SegundoApellido = jefe.SegundoApellido,
                Email = jefe.Email,
                UnidadTecnica = jefe.UnidadTecnica,
                Categoria = jefe.Categoria,
                EstaActivo = jefe.EstaActivo,
                FechaIngreso = jefe.FechaIngreso,
                PhoneNumber = jefe.PhoneNumber
            };
        }

        public UserViewModel ObtenerDirectorGeneral()
        {
            var directorRole = _roleManager.FindByName("Director");
            var directorGeneralCategoria = _categoriaServicio.ObtenerCategoriaPorNombre("director general");

            var director = _userManager.Users
                .Where(x => x.CategoriaId == directorGeneralCategoria.CategoriaId && 
                    x.Roles.Any(r => r.RoleId == directorRole.Id))
                .FirstOrDefault();

            if (director == null)
                throw new Exception("No se encontró ningún director general");

            return new UserViewModel()
            {
                Id = director.Id,
                Nombre = director.Nombre,
                PrimerApellido = director.PrimerApellido,
                SegundoApellido = director.SegundoApellido,
                Email = director.Email,
                UnidadTecnica = director.UnidadTecnica,
                Categoria = director.Categoria,
                EstaActivo = director.EstaActivo,
                FechaIngreso = director.FechaIngreso,
                PhoneNumber = director.PhoneNumber
            };
        }

        public bool EstaActivo(string email)
        {
            var usuario = _userManager.FindByEmail(email);
            return usuario.EstaActivo;
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
                EstaActivo = x.EstaActivo,
                EsSuperusuario = _userManager.IsInRole(x.Id, "Manager")
            }).ToList();
        }

        public bool EsSuperusuario(string id)
        {
            return _userManager.IsInRole(id, "Manager");
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
                EsSuperusuario = _userManager.IsInRole(user.Id, "Manager"),
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

                // Guardar foto de usuario, en caso que se haya seleccionado alguna
                GuardarFoto(usuario.Foto);

                // Reemplazo el rol del usuario en caso que se haya seleccionado uno diferente
                var roleId = user.Roles.FirstOrDefault().RoleId;
                var role = _roleManager.Roles.FirstOrDefault(r => r.Id == roleId);
                var newRole = _roleManager.Roles.FirstOrDefault(r => r.Id == usuario.SelectedRoleId);

                if (newRole.Id != role.Id)
                {
                    _userManager.RemoveFromRole(user.Id, role.Name);
                    _userManager.AddToRole(user.Id, newRole.Name);
                }

                if(usuario.EsSuperusuario && !_userManager.IsInRole(user.Id, "Manager"))
                    _userManager.AddToRole(user.Id, "Manager");

                if (!usuario.EsSuperusuario && _userManager.IsInRole(user.Id, "Manager"))
                    _userManager.RemoveFromRole(user.Id, "Manager");

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
                user.FotoRuta = usuario.Foto != null ? Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"), usuario.Foto.FileName) : null;

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

        public async Task<UserViewModel> GetLoggedInUser()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());
            var role = user.Roles.Count == 1 ?
                user.Roles.FirstOrDefault() :
                user.Roles.FirstOrDefault(r => !_roleManager.FindById(r.RoleId).Name.Equals("manager", StringComparison.OrdinalIgnoreCase));
                
            return new UserViewModel()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                Categoria = user.Categoria,
                EsSuperusuario = EsSuperusuario(user.Id),
                EstaActivo = user.EstaActivo,
                FechaIngreso = user.FechaIngreso,
                //Foto = user.FotoRuta
                FotoRuta = user.FotoRuta,
                PhoneNumber = user.PhoneNumber,
                UnidadTecnica = user.UnidadTecnica,
                Role = _roleManager.FindById(role.RoleId)
            };
        }

        public async Task<bool> Crear(UserRolesUnidadCategoria usuario)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(usuario.SelectedRoleId);
                var contrasenaTemporal = Membership.GeneratePassword(15, 2);

                var result = await _userManager.CreateAsync(new ApplicationUser()
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    PrimerApellido = usuario.PrimerApellido,
                    SegundoApellido = usuario.SegundoApellido,
                    Email = usuario.Email,
                    PhoneNumber = usuario.PhoneNumber,
                    UserName = usuario.Email,
                    FechaIngreso = usuario.FechaIngreso,
                    UnidadTecnicaId = Convert.ToInt32(usuario.SelectedUnidadTecnicaId),
                    CategoriaId = Convert.ToInt32(usuario.SelectedCategoriaId),
                    EstaActivo = true,
                    EsContrasenaTemporal = true,
                    FotoRuta = usuario.Foto != null ? Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"), usuario.Foto.FileName) : null
                }, contrasenaTemporal);

                if (result.Succeeded)
                {
                    var userSaved = await _userManager.FindByEmailAsync(usuario.Email);
                    await _userManager.AddToRoleAsync(userSaved.Id.ToString(), role.Name);

                    if (usuario.EsSuperusuario)
                        await _userManager.AddToRoleAsync(userSaved.Id.ToString(), "Manager");

                    // Guardar foto de usuario, en caso que se haya seleccionado alguna
                    GuardarFoto(usuario.Foto);

                    // Envio contrasena temporal al correo del usuario
                    await _userManager.SendEmailAsync(userSaved.Id, "Contraseña Temporal", $"Su contraseña temporal para el sistema de solicitud de vacaciones es: <strong>{ contrasenaTemporal }</strong>");

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
    }
}