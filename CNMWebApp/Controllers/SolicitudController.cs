using CNMWebApp.Authorization;
using CNMWebApp.Models;
using CNMWebApp.Services;
using Microsoft.AspNet.Identity;
using PagedList;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Controllers
{
    [Auth(Roles = "Funcionario, Director, Jefatura, Recursos Humanos, Manager")]
    public class SolicitudController : Controller
    {
        private SolicitudService solicitudService;
        private UserService userService;

        public SolicitudController()
        {
            solicitudService = new SolicitudService();
            userService = new UserService();
        }

        // GET: Solicitud
        public async Task<ActionResult> Index(string filtro, int? pagina, bool misSolicitudes = true)
        {
            ViewBag.MisSolicitudes = misSolicitudes;

            var user = await userService.GetLoggedInUser();

            var solicitudes = solicitudService.ObtenerSolicitudes(user, misSolicitudes);

            if (!string.IsNullOrEmpty(filtro))
            {
                solicitudes = FiltrarSolicitudes(solicitudes, filtro);
            }

            int tamanoPagina = 10;
            int numeroPagina = (pagina ?? 1);

            return View(solicitudes.ToPagedList(numeroPagina, tamanoPagina));
        }

        // GET: Solicitud/Details/5
        public async Task<ActionResult> Detalles(int id)
        {
            // Generate PDF for sample purposes
            var user = await userService.GetLoggedInUser();
            //var solicitudes = solicitudService.ObtenerSolicitudes(user);
            //var model = solicitudes.FirstOrDefault(x => x.SolicitudVacacionesId == id);

            return null;
            //return View(model);
        }

        public async Task<ActionResult> GeneratePDF()
        {
            var user = await userService.GetLoggedInUser();
            var solicitudes = solicitudService.ObtenerSolicitudes(user, true);

            var actionPDF = new Rotativa.ViewAsPdf("Detalles", solicitudes.First())
            {
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = new Margins(12, 12, 12, 12), // it’s in millimeters
                PageWidth = 180,
                PageHeight = 297
                //CustomSwitches = "–outline –print-media-type –footer-center ”Confidential” –footer-right[page]/[toPage] –footer-left[date]"
            };
            byte[] pdf = actionPDF.BuildFile(ControllerContext);
            using (FileStream stream = new FileStream(Server.MapPath("~/PDFs/File.pdf"), FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.Write(pdf, 0, pdf.Length);
            }

            return null;
        }

        // GET: Solicitud/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Solicitud/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Solicitud/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Solicitud/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Solicitud/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Solicitud/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private List<SolicitudVacaciones> FiltrarSolicitudes(IEnumerable<SolicitudVacaciones> solicitudes, string filtro)
        {
            filtro = filtro.ToLower();

            var solicitudesFiltradas = solicitudes
                .Where(x => x.Comentario.ToLower().Contains(filtro) ||
                 x.UsuarioId.ToLower().Contains(filtro) ||
                 x.Estado.Nombre.ToLower().Contains(filtro))
                .ToList();

            return solicitudesFiltradas;
        }
    }
}
