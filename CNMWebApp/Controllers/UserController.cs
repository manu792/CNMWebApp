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
        private static UserRolesViewModel userInfo;

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
            var unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas();

            return View(new UserRolesViewModel()
            {
                Roles = roles.ToList(),
                UnidadesTecnicas = unidadesTecnicas.ToList()
            });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserRolesViewModel user)
        {
            userInfo = user;

            if (!ModelState.IsValid)
            {
                user.Roles = _roleService.GetAllRoles().ToList();
                user.UnidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

                return View(user);
            }

            return RedirectToAction("CreateNext");

            //user.Roles = _roleService.GetAllRoles().ToList();
            //user.Categorias = _categoriaServicio.ObtenerCategorias().ToList();
            //user.UnidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            //ModelState.AddModelError("", "Hubo un problema al tratar de crear al usuario. Por favor contacte a soporte si sigue teniendo este problema.");
            //return View(user);
        }

        // GET: CreateNext
        public ActionResult CreateNext()
        {
            var user = userInfo;

            var categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(user.SelectedRoleId);

            return View(new UserRolesViewModel()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                SelectedRoleId = user.SelectedRoleId,
                SelectedUnidadTecnicaId = user.SelectedUnidadTecnicaId,
                Categorias = categorias.ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateNext(UserRolesViewModel user)
        {
            var userTemp = userInfo;

            userTemp.PhoneNumber = user.PhoneNumber;
            userTemp.SelectedCategoriaId = user.SelectedCategoriaId;
            userTemp.FechaIngreso = user.FechaIngreso;

            if (TryValidateModel(userTemp))
                return View(user);

            var succeeded = await _userService.Create(user);
            if (succeeded)
                return RedirectToAction("CreateNext");

            ModelState.AddModelError("", "Hubo un problema al tratar de crear al usuario. Por favor contacte a soporte si sigue teniendo este problema.");
            return View(user);
        }
    }
}