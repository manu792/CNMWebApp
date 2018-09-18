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
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private UserService _userService;
        private RoleService _roleService;
        private CategoriaServicio _categoriaServicio;
        private UnidadTecnicaServicio _unidadTecnicaServicio;

        public UserController()
        {
            _userService = new UserService();
            _roleService = new RoleService();
            _categoriaServicio = new CategoriaServicio();
            _unidadTecnicaServicio = new UnidadTecnicaServicio();
        }
        // GET: User
        public ActionResult Index()
        {
            var users = _userService.GetUsers();
            return View(users);
        }

        // GET: Create
        public ActionResult Create()
        {
            var roles = _roleService.GetAllRoles();
            var categorias = _categoriaServicio.ObtenerCategorias();
            var unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas();
            var jefes = _userService.ObtenerJefes();

            return View(new UserRolesViewModel()
            {
                Roles = roles.ToList(),
                UnidadesTecnicas = unidadesTecnicas.ToList(),
                Categorias = categorias.ToList(),
                Jefes = jefes.ToList()
            });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserRolesViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            await _userService.Create(user);

            return null;
        }
    }
}