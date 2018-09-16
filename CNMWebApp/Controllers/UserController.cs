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

        public UserController()
        {
            _userService = new UserService();
            _roleService = new RoleService();
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
            var viewModel = _roleService.GetAllRoles();
            return View(viewModel);
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