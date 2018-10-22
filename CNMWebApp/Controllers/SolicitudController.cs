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
        public async Task<ActionResult> Index(string filtro, int? pagina, string filtrarPor, string fechaInicio, string fechaFinal)
        {
            ViewBag.FiltradoPor = filtrarPor;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFinal = fechaFinal;

            var user = await userService.GetLoggedInUser();

            var solicitudes = ObtenerSolicitudesPorFiltro(filtrarPor, user, fechaInicio, fechaFinal);

            if (!string.IsNullOrEmpty(filtro))
            {
                solicitudes = FiltrarSolicitudes(solicitudes, filtro);
            }

            int tamanoPagina = 10;
            int numeroPagina = (pagina ?? 1);

            return View(solicitudes.ToPagedList(numeroPagina, tamanoPagina));
        }

        private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorFiltro(string filtrarPor, UserViewModel user, string fechaInicio, string fechaFinal)
        {
            if (filtrarPor == null)
                return solicitudService.ObtenerMisSolicitudes(user, fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("funcionarios", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudesFuncionarios(fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("jefaturas", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudesJefaturas(fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudesRH(fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("directorgeneral", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudesDirectorGeneral(fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("directoradministrativo", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudesDirectorAdministrativo(fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("todos", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerSolicitudes(fechaInicio, fechaFinal).ToList();

            return solicitudService.ObtenerMisSolicitudes(user, fechaInicio, fechaFinal);
        }

        // GET Solicitud/Detalle/1
        public ActionResult Detalle(Guid id)
        {
            var solicitud = solicitudService.ObtenerSolicitudPorId(id);
            if (solicitud == null)
                return HttpNotFound("ID de solicitud no encotrada.");

            var annosLaborados = DateTime.Now.Year - solicitud.Usuario.FechaIngreso.Year;
            if (solicitud.Usuario.FechaIngreso > DateTime.Now.AddYears(-annosLaborados)) annosLaborados--;

            return View(new SolicitudViewModel()
            {
                SolicitudId = solicitud.SolicitudVacacionesId,
                UsuarioId = solicitud.Usuario.Id,
                Nombre = solicitud.Usuario.Nombre,
                PrimerApellido = solicitud.Usuario.PrimerApellido,
                SegundoApellido = solicitud.Usuario.SegundoApellido,
                Email = solicitud.Usuario.Email,
                PhoneNumber = solicitud.Usuario.PhoneNumber,
                UnidadTecnica = solicitud.Usuario.UnidadTecnica,
                Categoria = solicitud.Usuario.Categoria,
                FechaIngreso = solicitud.Usuario.FechaIngreso,
                EstaActivo = solicitud.Usuario.EstaActivo,
                CantidadAnnosLaborados = annosLaborados < 0 ? 0 : annosLaborados,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                Comentario = solicitud.Comentario,
                DiasPorSolicitud = solicitud.DiasPorSolicitud.Select(s => new DiasPorSolicitudViewModel()
                {
                    UsuarioId = s.UsuarioId,
                    Fecha = s.Fecha.ToString("yyyy-MM-dd"),
                    Periodo = s.Periodo
                }).ToList(),
                SaldoDiasDisponibles = solicitud.Usuario.SaldoDiasEmpleado.SaldoDiasDisponibles
            });
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
            var solicitudes = solicitudService.ObtenerMisSolicitudes(user, null, null);

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

                // Info viene de BD
                SaldoDiasDisponibles = usuario.SaldoDiasDisponibles
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

            if(solicitudVacaciones.CantidadDiasSolicitados > solicitudVacaciones.SaldoDiasDisponibles)
            {
                ModelState.AddModelError("", "La cantidad de días solicitados no puede ser mayor al saldo de días disponibles.");
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

                return RedirectToAction("Index", "Solicitud");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
                return View(solicitudVacaciones);
            }
        }

        // GET: Solicitud/CrearANombreDeEmpleado
        [Auth(Roles = "Manager, Recursos Humanos")]
        public ActionResult CrearParaEmpleado()
        {
            var empleados = userService.GetUsers();

            return View(new SolicitudParaEmpleado()
            {
                Colaboradores = empleados.ToList()
            });
        }

        // POST: Solicitud/CrearANombreDeEmpleado
        [HttpPost]
        [Auth(Roles = "Manager, Recursos Humanos")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearParaEmpleado(SolicitudParaEmpleado solicitudVacaciones)
        {
            var empleados = userService.GetUsers();
            solicitudVacaciones.Colaboradores = empleados.ToList();

            if (!ModelState.IsValid)
            {
                return View(solicitudVacaciones);
            }

            //if (solicitudVacaciones.CantidadDiasSolicitados > solicitudVacaciones.SaldoDiasDisponibles)
            //{
            //    ModelState.AddModelError("", "La cantidad de días solicitados no puede ser mayor al saldo de días disponibles.");
            //    return View(solicitudVacaciones);
            //}

            try
            {
                var rowsAffected = await solicitudService.CrearSolicitudVacaciones(solicitudVacaciones);

                if (rowsAffected <= 0)
                {
                    ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
                    return View(solicitudVacaciones);
                }

                return View(solicitudVacaciones);
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

        [Auth(Roles = "Jefatura, Director")]
        public async Task<ActionResult> Revisar(string id)
        {
            Guid guid;

            var usuario = await userService.GetLoggedInUser();

            if (!Guid.TryParse(id, out guid))
                return RedirectToAction("SolicitudesEmpleados");

            var solicitudId = guid;

            var solicitud = solicitudService.ObtenerSolicitudPorId(solicitudId);

            if (solicitud == null || solicitud.UsuarioId == usuario.Id)
                return HttpNotFound($"La solicitud con id {id} no existe");

            var annosLaborados = DateTime.Now.Year - solicitud.Usuario.FechaIngreso.Year;
            if (solicitud.Usuario.FechaIngreso > DateTime.Now.AddYears(-annosLaborados)) annosLaborados--;

            return View(new SolicitudViewModel()
            {
                SolicitudId = solicitud.SolicitudVacacionesId,
                UsuarioId = solicitud.Usuario.Id,
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
                Comentario = solicitud.Comentario,
                DiasPorSolicitud = solicitud.DiasPorSolicitud.Select(s => new DiasPorSolicitudViewModel()
                {
                    UsuarioId = s.UsuarioId,
                    Fecha = s.Fecha.ToString("yyyy-MM-dd"),
                    Periodo = s.Periodo
                }).ToList(),

                // Info viene de BD
                SaldoDiasDisponibles = solicitud.Usuario.SaldoDiasEmpleado.SaldoDiasDisponibles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(Roles = "Jefatura, Director")]
        [MultipleButton(Name = "action", Argument = "Aprobar")]
        public async Task<ActionResult> Aprobar(SolicitudViewModel solicitud)
        {
            try
            {
                var usuario = await userService.GetLoggedInUser();

                var rowsAffected = await solicitudService.Aprobar(solicitud.SolicitudId, solicitud.ComentarioJefatura, usuario, solicitud.UsuarioId);
                if(rowsAffected > 0)
                {
                    return RedirectToAction("SolicitudesEmpleados");
                }

                ModelState.AddModelError("", $"No se encontró ninguna solicitud con el id {solicitud.SolicitudId}");
                return RedirectToAction("Revisar", new { id = solicitud.SolicitudId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hubo un problema al tratar de procesar la solicitud. Favor contacte a soporte si el problem persiste");
                return RedirectToAction("Revisar", new { id = solicitud.SolicitudId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(Roles = "Jefatura, Director")]
        [MultipleButton(Name = "action", Argument = "Rechazar")]
        public async Task<ActionResult> Rechazar(SolicitudViewModel solicitud)
        {
            try
            {
                var usuario = await userService.GetLoggedInUser();

                var rowsAffected = solicitudService.Rechazar(solicitud.SolicitudId, solicitud.ComentarioJefatura, usuario, solicitud.UsuarioId);
                if (rowsAffected > 0)
                {
                    return RedirectToAction("SolicitudesEmpleados");
                }

                ModelState.AddModelError("", $"No se encontró ninguna solicitud con el id {solicitud.SolicitudId}");
                return RedirectToAction("Revisar", new { id = solicitud.SolicitudId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hubo un problema al tratar de procesar la solicitud. Favor contacte a soporte si el problem persiste");
                return RedirectToAction("Revisar", new { id = solicitud.SolicitudId });
            }
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
