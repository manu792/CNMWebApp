using CNMWebApp.Authorization;
using CNMWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Auth(Roles = "Manager")]
    public class AdminController : Controller
    {
        public AdminController()
        {
            
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}