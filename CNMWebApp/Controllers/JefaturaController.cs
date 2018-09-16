using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Authorize(Roles = "Jefatura")]
    public class JefaturaController : Controller
    {
        // GET: Jefatura
        public ActionResult Index()
        {
            return View();
        }
    }
}
