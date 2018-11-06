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
        private RoleService _roleService;
        private ApplicationDbContext _context;
        private static UserViewModel loggedUser;
        

        public UserService()
        {
            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            _categoriaServicio = new CategoriaServicio();
            _roleService = new RoleService();
            _context = new ApplicationDbContext();
        }

        public UserViewModel ObtenerJefePorEmpleadoId(string empleadoId)
        {
            var jefaturaRole = _roleManager.FindByName("Jefatura");
            var jefe = _userManager.Users
                .Where(x => x.Id == empleadoId && x.Roles.Any(r => r.RoleId == jefaturaRole.Id))
                .FirstOrDefault();

            if (jefe == null)
                return null;

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
                FechaCreacion = jefe.FechaCreacion,
                PhoneNumber = jefe.PhoneNumber,
                SaldoDiasDisponibles = jefe.SaldoDiasEmpleado.SaldoDiasDisponibles
            };
        }
        public IEnumerable<UserViewModel> ObtenerJefesPorUnidadTecnica(int unidadTecnicaId)
        {
            var jefaturaRole = _roleManager.FindByName("Jefatura");
            var jefes = _userManager.Users
                .Where(x => x.UnidadTecnicaId == unidadTecnicaId && x.Roles.Any(r => r.RoleId == jefaturaRole.Id));

            return jefes.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Nombre = x.Nombre,
                PrimerApellido = x.PrimerApellido,
                SegundoApellido = x.SegundoApellido,
                Email = x.Email,
                UnidadTecnica = x.UnidadTecnica,
                Categoria = x.Categoria,
                EstaActivo = x.EstaActivo,
                FechaIngreso = x.FechaIngreso,
                FechaCreacion = x.FechaCreacion,
                PhoneNumber = x.PhoneNumber,
                SaldoDiasDisponibles = x.SaldoDiasEmpleado.SaldoDiasDisponibles
            });
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
                return null;

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
                FechaCreacion = director.FechaCreacion,
                PhoneNumber = director.PhoneNumber,
                SaldoDiasDisponibles = director.SaldoDiasEmpleado.SaldoDiasDisponibles
            };
        }

        public UserViewModel ObtenerDirectorAdministrativo()
        {
            var directorRole = _roleManager.FindByName("Director");
            var directorAdministrativoCategoria = _categoriaServicio.ObtenerCategoriaPorNombre("director administrativo");

            var director = _userManager.Users
                .Where(x => x.CategoriaId == directorAdministrativoCategoria.CategoriaId &&
                    x.Roles.Any(r => r.RoleId == directorRole.Id))
                .FirstOrDefault();

            if (director == null)
                return null;

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
                FechaCreacion = director.FechaCreacion,
                PhoneNumber = director.PhoneNumber,
                SaldoDiasDisponibles = director.SaldoDiasEmpleado.SaldoDiasDisponibles
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
                FechaCreacion = x.FechaCreacion,
                Role = _roleManager.FindById(x.Roles.First().RoleId.ToString()),
                UnidadTecnica = x.UnidadTecnica,
                Categoria = x.Categoria,
                EstaActivo = x.EstaActivo,
                EsSuperusuario = _userManager.IsInRole(x.Id, "Manager"),
                SaldoDiasDisponibles = x.SaldoDiasEmpleado.SaldoDiasDisponibles,
                JefeId = x.JefeId
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
                return null;

            return new UserViewModel()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FechaIngreso = user.FechaIngreso,
                FechaCreacion = user.FechaCreacion,
                Role = _roleManager.FindById(user.Roles.First().RoleId.ToString()),
                UnidadTecnica = user.UnidadTecnica,
                Categoria = user.Categoria,
                EstaActivo = user.EstaActivo,
                EsSuperusuario = _userManager.IsInRole(user.Id, "Manager"),
                FotoRuta = user.FotoRuta,
                SaldoDiasDisponibles = user.SaldoDiasEmpleado.SaldoDiasDisponibles,
                JefeId = user.JefeId
            };
        }

        public bool ActualizarUsuario(string id, UserRolesUnidadCategoria usuario)
        {
            try
            {
                string fotoRuta = usuario.FotoRuta;

                if (usuario.Foto != null)
                    fotoRuta = usuario.Foto.FileName;
                
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
                user.FotoRuta = string.IsNullOrEmpty(fotoRuta) ? null : fotoRuta;
                user.SaldoDiasEmpleado.SaldoDiasDisponibles = usuario.SaldoDiasDisponibles;
                user.JefeId = usuario.JefeId;

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
                if (usuario == null)
                    return false;

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

        public async Task<string> ObtenerFoto()
        {
            var usuario = await GetLoggedInUser();
            if (string.IsNullOrEmpty(usuario.FotoRuta))
                return "CNM.png";

            // var fotoUrl = HttpContext.Current.Request.Url.c.Content("~/Content/Images/Logo.png");
            // var fotoUrl = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Fotos"), usuario.FotoRuta);
            return usuario.FotoRuta;
        }

        public async Task<UserViewModel> GetLoggedInUser()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());
            if(user == null)
                return null;

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
                FechaCreacion = user.FechaCreacion,
                FotoRuta = user.FotoRuta,
                PhoneNumber = user.PhoneNumber,
                UnidadTecnica = user.UnidadTecnica,
                Role = _roleManager.FindById(role.RoleId),
                SaldoDiasDisponibles = user.SaldoDiasEmpleado != null ? user.SaldoDiasEmpleado.SaldoDiasDisponibles : 0
            };
        }

        public UserViewModel ObtenerUsuarioLogueado()
        {
            if (loggedUser != null)
                return loggedUser;

            string currentUserId = HttpContext.Current.User.Identity.GetUserId();
            var user = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
            var role = user.Roles.Count == 1 ?
                user.Roles.FirstOrDefault() :
                user.Roles.FirstOrDefault(r => !_roleManager.FindById(r.RoleId).Name.Equals("manager", StringComparison.OrdinalIgnoreCase));

            loggedUser = new UserViewModel()
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
                FechaCreacion = user.FechaCreacion,
                FotoRuta = user.FotoRuta,
                PhoneNumber = user.PhoneNumber,
                UnidadTecnica = user.UnidadTecnica,
                Role = _roleManager.FindById(role.RoleId),
                SaldoDiasDisponibles = user.SaldoDiasEmpleado != null ? user.SaldoDiasEmpleado.SaldoDiasDisponibles : 0
            };

            return loggedUser;
        }

        public void SignOut()
        {
            loggedUser = null;
        }

        public async Task<bool> Crear(UserRolesUnidadCategoria usuario)
        {
            VerificarExistenciaJefaturas(usuario);

            try
            {
                var role = await _roleManager.FindByIdAsync(usuario.SelectedRoleId);
                var contrasenaTemporal = Membership.GeneratePassword(15, 2);
                var newUser = new ApplicationUser()
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    PrimerApellido = usuario.PrimerApellido,
                    SegundoApellido = usuario.SegundoApellido,
                    Email = usuario.Email,
                    PhoneNumber = usuario.PhoneNumber,
                    UserName = usuario.Email,
                    FechaIngreso = usuario.FechaIngreso,
                    FechaCreacion = DateTime.Now,
                    UnidadTecnicaId = Convert.ToInt32(usuario.SelectedUnidadTecnicaId),
                    CategoriaId = Convert.ToInt32(usuario.SelectedCategoriaId),
                    EstaActivo = true,
                    EsContrasenaTemporal = true,
                    FotoRuta = usuario.Foto != null ? usuario.Foto.FileName : null,
                    SaldoDiasEmpleado = new SaldoDiasPorEmpleado()
                    {
                        Cedula = usuario.Id,
                        SaldoDiasDisponibles = 0,
                        UltimaActualizacion = DateTime.Now
                    },
                    JefeId = usuario.JefeId
                };
                
                var result = await _userManager.CreateAsync(newUser, contrasenaTemporal);

                if (result.Succeeded)
                {
                    var userSaved = await _userManager.FindByEmailAsync(usuario.Email);
                    await _userManager.AddToRoleAsync(userSaved.Id.ToString(), role.Name);

                    if (usuario.EsSuperusuario)
                        await _userManager.AddToRoleAsync(userSaved.Id.ToString(), "Manager");

                    // Guardar foto de usuario, en caso que se haya seleccionado alguna
                    GuardarFoto(usuario.Foto);

                    // Envio contrasena temporal al correo del usuario
                    try
                    {
                        await _userManager.SendEmailAsync(userSaved.Id, "Contraseña Temporal", $"Su contraseña temporal para el sistema de solicitud de vacaciones es: <strong>{ contrasenaTemporal }</strong>");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void VerificarExistenciaJefaturas(UserRolesUnidadCategoria usuario)
        {
            var jefaturaRole = _roleService.ObtenerRolPorNombre("Jefatura");
            var directorRole = _roleService.ObtenerRolPorNombre("Director");

            //if (usuario.SelectedRoleId == jefaturaRole.Id)
            //{
            //    var jefe = ObtenerJefePorUnidadTecnica(usuario.Id);
            //    if (jefe != null)
            //        throw new Exception("Ya existe el puesto de jefatura para la unidad técnica seleccionada");
            //}

            if (usuario.SelectedRoleId == directorRole.Id)
            {
                var directorGeneral = ObtenerDirectorGeneral();
                var directorAdministrativo = ObtenerDirectorAdministrativo();

                if (directorGeneral != null)
                {
                    if (directorGeneral.Categoria.CategoriaId == Convert.ToInt32(usuario.SelectedCategoriaId))
                        throw new Exception("Ya existe el puesto de director general");
                }

                if (directorAdministrativo != null)
                {
                    if (directorAdministrativo.Categoria.CategoriaId == Convert.ToInt32(usuario.SelectedCategoriaId))
                        throw new Exception("Ya existe el puesto de director administrativo");
                }
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