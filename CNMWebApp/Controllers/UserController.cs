using CNMWebApp.Models;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private UserService _userService;

        public UserController()
        {
            _userService = new UserService();
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
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                
            }

            return null;
        }
    }
}