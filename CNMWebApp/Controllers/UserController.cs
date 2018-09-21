using CNMWebApp.Models;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CNMWebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private UserService _userServicio;
        private RoleService _roleServicio;
        private CategoriaServicio _categoriaServicio;
        private UnidadTecnicaServicio _unidadTecnicaServicio;

        public UserController()
        {
            _userServicio = new UserService();
            _roleServicio = new RoleService();
            _categoriaServicio = new CategoriaServicio();
            _unidadTecnicaServicio = new UnidadTecnicaServicio();
        }

        [HttpGet]
        public JsonResult ObtenerCategoriasPorRoleId(string roleId)
        {
            var categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(roleId).ToList();
            var listaCategorias = new SelectList(categorias, "CategoriaId", "Nombre", 0);
            return Json(listaCategorias, JsonRequestBehavior.AllowGet);
        }

        // GET: User
        public ActionResult Index(string filtro, int? pagina)
        {
            var users = _userServicio.GetUsers();

            if (!string.IsNullOrEmpty(filtro))
            {
                users = FiltrarUsuarios(users, filtro);
            }

            int tamanoPagina = 10;
            int numeroPagina = (pagina ?? 1);

            return View(users.ToPagedList(numeroPagina, tamanoPagina));
        }

        // GET: Create
        public ActionResult Create()
        {
            var roles = _roleServicio.GetAllRoles();
            //var categorias = _categoriaServicio.ObtenerCategorias().ToList();
            var unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            return View(new UserRolesUnidadCategoria()
            {
                Roles = roles.ToList(),
                //Categorias = categorias.ToList(),
                UnidadesTecnicas = unidadesTecnicas.ToList()
            });
        }

       // POST: Create
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserRolesUnidadCategoria user)
        {
            IEnumerable<IdentityRole> roles;
            IEnumerable<Categoria> categorias;
            IEnumerable<UnidadTecnica> unidadesTecnicas;

            if (!ModelState.IsValid)
            {
                roles = _roleServicio.GetAllRoles();
                categorias = _categoriaServicio.ObtenerCategorias().ToList();
                unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

                user.Roles = roles.ToList();
                user.Categorias = categorias.ToList();
                user.UnidadesTecnicas = unidadesTecnicas.ToList();

                return View(user);
            }

            var succeeded = await _userServicio.Create(user);
            if (succeeded)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Hubo un problema al tratar de crear al usuario. Por favor contacte a soporte si sigue teniendo este problema.");

            roles = _roleServicio.GetAllRoles();
            categorias = _categoriaServicio.ObtenerCategorias().ToList();
            unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            user.Roles = roles.ToList();
            user.Categorias = categorias.ToList();
            user.UnidadesTecnicas = unidadesTecnicas.ToList();

            return View(user);
        }

        // GET: Editar/5
        public ActionResult Editar(string id)
        {
            var usuario = _userServicio.ObtenerUsuarioPorId(id);
            var categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(usuario.Role.Id).ToList();
            var roles = _roleServicio.GetAllRoles().ToList();
            var unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            return View(new UserRolesUnidadCategoria()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                Email = usuario.Email,
                FechaIngreso = usuario.FechaIngreso,
                //Foto = usuario.Foto,
                PhoneNumber = usuario.PhoneNumber,
                EstaActivo = usuario.EstaActivo,
                Categorias = categorias,
                Roles = roles,
                UnidadesTecnicas = unidadesTecnicas,
                Role = usuario.Role,
                Categoria = usuario.Categoria,
                UnidadTecnica = usuario.UnidadTecnica,
                FotoRuta = usuario.FotoRuta
            });
        }

        // POST: User/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UserRolesUnidadCategoria usuario)
        {
            IEnumerable<IdentityRole> roles;
            IEnumerable<Categoria> categorias;
            IEnumerable<UnidadTecnica> unidadesTecnicas;
            IdentityRole role;

            if (!ModelState.IsValid)
            {
                roles = _roleServicio.GetAllRoles();
                role = _roleServicio.ObtenerRolPorId(usuario.SelectedRoleId);
                categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(role.Id).ToList();
                unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

                usuario.Roles = roles.ToList();
                usuario.Categorias = categorias.ToList();
                usuario.UnidadesTecnicas = unidadesTecnicas.ToList();
                usuario.Role = _userServicio.ObtenerUsuarioPorId(usuario.Id).Role;

                return View(usuario);
            }

            var resultado = _userServicio.ActualizarUsuario(usuario.Id, usuario);
            if (resultado)
            {
                return RedirectToAction("Index");
            }

            roles = _roleServicio.GetAllRoles();
            role = _roleServicio.ObtenerRolPorId(usuario.SelectedRoleId);
            categorias = _categoriaServicio.ObtenerCategoriasPorRoleId(role.Id).ToList();
            unidadesTecnicas = _unidadTecnicaServicio.ObtenerUnidadesTecnicas().ToList();

            usuario.Roles = roles.ToList();
            usuario.Categorias = categorias.ToList();
            usuario.UnidadesTecnicas = unidadesTecnicas.ToList();
            usuario.Role = _userServicio.ObtenerUsuarioPorId(usuario.Id).Role;

            return View(usuario);
        }
        // GET: User/Eliminar
        public ActionResult Eliminar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var usuario = _userServicio.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: User/Eliminar
        [HttpPost]
        [ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(string id)
        {
            var usuario = _userServicio.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            var fueEliminado = _userServicio.EliminarUsuario(id);
            if(fueEliminado)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Hubo un problema al tratar de eliminar el usuario. Por favor contacte a soporte si sigue ocurriendo");
            return View(usuario);
        }

        private List<UserViewModel> FiltrarUsuarios(IEnumerable<UserViewModel> users, string filtro)
        {
            filtro = filtro.ToLower();

            var usuariosFiltrados = users
                .Where(x => x.Id.ToLower().Contains(filtro) ||
                 x.Nombre.ToLower().Contains(filtro) ||
                 x.PrimerApellido.ToLower().Contains(filtro) ||
                 (!string.IsNullOrEmpty(x.SegundoApellido) ? x.SegundoApellido.ToLower().Contains(filtro) : false) ||
                 x.Email.ToLower().Contains(filtro) ||
                 (!string.IsNullOrEmpty(x.PhoneNumber) ? x.PhoneNumber.ToLower().Contains(filtro): false) ||
                 x.FechaIngreso.ToString().Contains(filtro) ||
                 x.Role.Name.ToLower().Contains(filtro) ||
                 x.UnidadTecnica.Nombre.ToLower().Contains(filtro) ||
                 x.Categoria.Nombre.ToLower().Contains(filtro))
                .ToList();

            return usuariosFiltrados;
        }
    }
}