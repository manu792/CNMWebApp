using CNMWebApp.Models;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CNMWebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private UserService _userServicio;
        private RoleService _roleServicio;
        private CategoriaServicio _categoriaServicio;
        private UnidadTecnicaServicio _unidadTecnicaServicio;
        private static UserRolesUnidadCategoria userInfo;

        public UserController()
        {
            _userServicio = new UserService();
            _roleServicio = new RoleService();
            _categoriaServicio = new CategoriaServicio();
            _unidadTecnicaServicio = new UnidadTecnicaServicio();
        }
        // GET: User
        public ActionResult Index(string filtro, int? pagina)
        {
            var users = _userServicio.GetUsers();

            int tamanoPagina = 10;
            int numeroPagina = (pagina ?? 1);

            return View(users.ToPagedList(numeroPagina, tamanoPagina));
        }

        // GET: Create
        public ActionResult Create()
        {
            var roles = _roleServicio.GetAllRoles();

            return View(new UserRolesViewModel()
            {
                Roles = roles.ToList()
            });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserRolesViewModel user)
        {
            userInfo = new UserRolesUnidadCategoria()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                SelectedRoleId = user.SelectedRoleId
            };

            if (!ModelState.IsValid)
            {
                user.Roles = _roleServicio.GetAllRoles().ToList();

                return View(user);
            }

            return RedirectToAction("CreateNext");
        }

        // GET: CreateNext
        public ActionResult CreateNext()
        {
            var user = userInfo;

            var categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(user.SelectedRoleId).ToList();
            var unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            return View(new UserRolesUnidadCategoria()
            {
                Id = user.Id,
                Nombre = user.Nombre,
                PrimerApellido = user.PrimerApellido,
                SegundoApellido = user.SegundoApellido,
                Email = user.Email,
                SelectedRoleId = user.SelectedRoleId,
                UnidadesTecnicas = unidadesTecnicas,
                Categorias = categorias,
                Role = _roleServicio.ObtenerRolPorId(user.SelectedRoleId)
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateNext(UserRolesUnidadCategoria user)
        {
            var userData = userInfo;
            IEnumerable<Categoria> categorias;
            IEnumerable<UnidadTecnica> unidadesTecnicas;

            userData.PhoneNumber = user.PhoneNumber;
            userData.SelectedCategoriaId = user.SelectedCategoriaId;
            userData.SelectedUnidadTecnicaId = user.SelectedUnidadTecnicaId;
            userData.FechaIngreso = user.FechaIngreso;
            userData.Role = _roleServicio.ObtenerRolPorId(userData.SelectedRoleId);
            if (userData.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase))
            {
                // Cambiar esto por un llamado a la base de datos donde me diga el ID segun el nombre
                userData.SelectedUnidadTecnicaId = "6";
                userData.SelectedCategoriaId = "9";
            }

            if (TryValidateModel(userData))
            {
                categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(user.SelectedRoleId);
                unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas();
                userData.Categorias = categorias.ToList();
                userData.UnidadesTecnicas = unidadesTecnicas.ToList();
                return View(userData);
            }
                

            var succeeded = await _userServicio.Create(userData);
            if (succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Hubo un problema al tratar de crear al usuario. Por favor contacte a soporte si sigue teniendo este problema.");
            categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(user.SelectedRoleId);
            unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas();
            userData.Categorias = categorias.ToList();
            userData.UnidadesTecnicas = unidadesTecnicas.ToList();
            return View(userData);
        }
    }
}