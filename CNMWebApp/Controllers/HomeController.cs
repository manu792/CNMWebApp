using CNMWebApp.Models;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserService _usuarioServicio;
        private RoleService _rolServicio;

        public HomeController()
        {
            _usuarioServicio = new UserService();
            _rolServicio = new RoleService();
        }

        public async Task<ActionResult> Index()
        {
            var usuario = await _usuarioServicio.GetLoggedInUser();
            
            //var role = usuario.Roles.Count == 1 ? usuario.Roles.FirstOrDefault() :
            //    usuario.Roles.FirstOrDefault(x => !_rolServicio.ObtenerRolPorId(x.RoleId).Name.Equals("manager", StringComparison.OrdinalIgnoreCase));

            return View(new UserViewModel()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                Role = usuario.Role,
                UnidadTecnica = usuario.UnidadTecnica,
                Categoria = usuario.Categoria,
                FechaIngreso = usuario.FechaIngreso,
                EstaActivo = usuario.EstaActivo,
                EsSuperusuario = _usuarioServicio.EsSuperusuario(usuario.Id),
            });
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}