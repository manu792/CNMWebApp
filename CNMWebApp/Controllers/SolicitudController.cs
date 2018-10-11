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
        public async Task<ActionResult> Index(string filtro, int? pagina)
        {
            var user = await userService.GetLoggedInUser();

            var solicitudes = solicitudService.ObtenerMisSolicitudes(user);

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
            var solicitudes = solicitudService.ObtenerMisSolicitudes(user);

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

        // GET: Solicitud/Crear
        public async Task<ActionResult> Crear()
        {
            // Consigo la informacion del usuario logeado para mostrar en el View
            var usuario = await userService.GetLoggedInUser();

            var annosLaborados = DateTime.Now.Year - usuario.FechaIngreso.Year;
            if (usuario.FechaIngreso > DateTime.Now.AddYears(-annosLaborados)) annosLaborados--;

            return View(new SolicitudViewModel()
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
                CantidadAnnosLaborados = annosLaborados < 0 ? 0 : annosLaborados,
                CantidadDiasSolicitados = 0,

                // Necesito la logica para saber calcular los dias disponibles segun fecha de ingreso 
                // y cantidad de vacaciones previamente solicitadas
                SaldoDiasDisponibles = 10
            });
        }

        // POST: Solicitud/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(SolicitudViewModel solicitudVacaciones)
        {
            if (!ModelState.IsValid)
            {
                return View(solicitudVacaciones);
            }

            try
            {
                var rowsAffected = await solicitudService.CrearSolicitudVacaciones(solicitudVacaciones);

                if (rowsAffected <= 0)
                {
                    ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
                    return View(solicitudVacaciones);
                }

                return RedirectToAction("Index", "Solicitud", new { misSolicitudes = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
                return View(solicitudVacaciones);
            }
        }
        
        [Auth(Roles = "Jefatura, Director")]
        public async Task<ActionResult> SolicitudesEmpleados(string filtro, int? pagina)
        {
            var jefe = await userService.GetLoggedInUser();

            var solicitudes = solicitudService.ObtenerSolicitudesPorAprobar(jefe);

            if (!string.IsNullOrEmpty(filtro))
            {
                solicitudes = FiltrarSolicitudes(solicitudes, filtro);
            }

            int tamanoPagina = 10;
            int numeroPagina = (pagina ?? 1);

            return View(solicitudes.ToPagedList(numeroPagina, tamanoPagina));
        }

        public ActionResult Revisar(int? id)
        {
            if (id == null)
                return RedirectToAction("SolicitudesEmpleados");

            var solicitudId = Convert.ToInt32(id);

            var solicitud = solicitudService.ObtenerSolicitudPorId(solicitudId);

            if (solicitud == null)
                return HttpNotFound($"La solicitud con id {id} no existe");

            var annosLaborados = DateTime.Now.Year - solicitud.Usuario.FechaIngreso.Year;
            if (solicitud.Usuario.FechaIngreso > DateTime.Now.AddYears(-annosLaborados)) annosLaborados--;

            return View(new SolicitudViewModel()
            {
                SolicitudId = solicitud.SolicitudVacacionesId,
                Id = solicitud.Usuario.Id,
                Nombre = solicitud.Usuario.Nombre,
                PrimerApellido = solicitud.Usuario.PrimerApellido,
                SegundoApellido = solicitud.Usuario.SegundoApellido,
                Email = solicitud.Usuario.Email,
                PhoneNumber = solicitud.Usuario.PhoneNumber,
                // Role = solicitud.Usuario.Role,
                UnidadTecnica = solicitud.Usuario.UnidadTecnica,
                Categoria = solicitud.Usuario.Categoria,
                FechaIngreso = solicitud.Usuario.FechaIngreso,
                EstaActivo = solicitud.Usuario.EstaActivo,
                CantidadAnnosLaborados = annosLaborados < 0 ? 0 : annosLaborados,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                DiasPorSolicitud = solicitud.DiasPorSolicitud.Select(s => new DiasPorSolicitudViewModel()
                {
                    UsuarioId = s.UsuarioId,
                    Fecha = s.Fecha.ToString("yyyy-MM-dd"),
                    Periodo = s.Periodo
                }).ToList(),

                // Necesito la logica para saber calcular los dias disponibles segun fecha de ingreso 
                // y cantidad de vacaciones previamente solicitadas
                SaldoDiasDisponibles = 10
            });
        }

        private List<SolicitudVacaciones> FiltrarSolicitudes(IEnumerable<SolicitudVacaciones> solicitudes, string filtro)
        {
            filtro = filtro.ToLower();

            var solicitudesFiltradas = solicitudes
                .Where(x => x.Comentario.ToLower().Contains(filtro) ||
                 x.UsuarioId.ToLower().Contains(filtro) ||
                 x.Estado.Nombre.ToLower().Contains(filtro) ||
                 x.Usuario.Nombre.ToLower().Contains(filtro) ||
                 x.Usuario.PrimerApellido.ToLower().Contains(filtro))
                .ToList();

            return solicitudesFiltradas;
        }
    }
}
