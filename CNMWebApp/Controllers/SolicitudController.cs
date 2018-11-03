using CNMWebApp.Authorization;
using CNMWebApp.Models;
using CNMWebApp.Services;
using Microsoft.AspNet.Identity;
using PagedList;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private EmailNotificationService emailNotification;
        private RoleService roleService;

        public SolicitudController()
        {
            solicitudService = new SolicitudService();
            userService = new UserService();
            emailNotification = new EmailNotificationService();
            roleService = new RoleService();
        }

        // GET: Solicitud
        public async Task<ActionResult> Index(string filtro, int? pagina, string filtrarPor, string fechaInicio, string fechaFinal)
        {
            ViewBag.FiltradoPor = filtrarPor;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFinal = fechaFinal;

            var user = await userService.GetLoggedInUser();
            if (user == null)
                return RedirectToAction("LogOut", "Account");

            try
            {
                var solicitudes = ObtenerSolicitudesPorFiltro(filtrarPor, user, fechaInicio, fechaFinal);

                if (!string.IsNullOrEmpty(filtro))
                {
                    solicitudes = FiltrarSolicitudes(solicitudes, filtro);
                }

                int tamanoPagina = 10;
                int numeroPagina = (pagina ?? 1);

                return View(solicitudes.ToPagedList(numeroPagina, tamanoPagina));
            }
            catch(Exception ex)
            {
                if (ex.Message.Equals("Rol inválido"))
                    return View("~/Views/Shared/Unauthorized.cshtml");
            }

            return View(new List<SolicitudVacaciones>());
        }

        private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorFiltro(string filtrarPor, UserViewModel user, string fechaInicio, string fechaFinal)
        {
            if (filtrarPor == null)
                return solicitudService.ObtenerMisSolicitudes(user, fechaInicio, fechaFinal).ToList();
            if (filtrarPor.Equals("funcionarios", StringComparison.OrdinalIgnoreCase))
            {
                if (!(user.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("Rol inválido");
                    
                return solicitudService.ObtenerSolicitudesFuncionarios(fechaInicio, fechaFinal).ToList();
            }
                
            if (filtrarPor.Equals("funcionariosporunidadtecnica", StringComparison.OrdinalIgnoreCase))
            {
                // El rol de funcionario es el unico que no puede ver 
                // solicitudes de funcionarios dentro de una unidad tecnica especifica

                if(user.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Rol inválido");

                return solicitudService.ObtenerSolicitudesFuncionariosPorUnidad(fechaInicio, fechaFinal, user).ToList();
            }
                
            if (filtrarPor.Equals("jefaturas", StringComparison.OrdinalIgnoreCase))
            {
                if (!(user.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("Rol inválido");

                return solicitudService.ObtenerSolicitudesJefaturas(fechaInicio, fechaFinal).ToList();
            }
                
            if (filtrarPor.Equals("recursoshumanos", StringComparison.OrdinalIgnoreCase))
            {
                if (!(user.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("Rol inválido");

                return solicitudService.ObtenerSolicitudesRH(fechaInicio, fechaFinal).ToList();
            }
                
            if (filtrarPor.Equals("directorgeneral", StringComparison.OrdinalIgnoreCase))
            {
                if (!(user.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("Rol inválido");

                return solicitudService.ObtenerSolicitudesDirectorGeneral(fechaInicio, fechaFinal).ToList();
            }
                
            if (filtrarPor.Equals("directoradministrativo", StringComparison.OrdinalIgnoreCase))
            {
                if (!(user.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase)))
                    throw new Exception("Rol inválido");

                return solicitudService.ObtenerSolicitudesDirectorAdministrativo(fechaInicio, fechaFinal).ToList();
            }
                
            if (filtrarPor.Equals("todos", StringComparison.OrdinalIgnoreCase))
                return solicitudService.ObtenerTodasSolicitudesPorRol(fechaInicio, fechaFinal, user).ToList();

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

        private string GeneratePDF(SolicitudViewModel solicitud)
        {
            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
            var id = solicitud.Id;
            var fileName = $"{id}_{date}";

            var actionPDF = new Rotativa.ViewAsPdf("PDFView", solicitud)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Margins(12, 25, 12, 25)
            };

            byte[] pdf = actionPDF.BuildFile(ControllerContext);
            using (FileStream stream = new FileStream(Server.MapPath($"~/PDFs/{fileName}.pdf"), FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.Write(pdf, 0, pdf.Length);
            }

            return $"{fileName}.pdf";
        }

        // GET: Solicitud/Crear
        public async Task<ActionResult> Crear()
        {
            // Consigo la informacion del usuario logeado para mostrar en el View
            var usuario = await userService.GetLoggedInUser();
            if (usuario == null)
                return RedirectToAction("LogOut", "Account");

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
            var solicitante = userService.ObtenerUsuarioPorId(solicitudVacaciones.Id);

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
                var guid = Guid.NewGuid();
                var rowsAffected = await solicitudService.CrearSolicitudVacaciones(solicitudVacaciones, guid);

                if (rowsAffected <= 0)
                {
                    ModelState.AddModelError("", "Hubo un problema al tratar de agregar la solicitud. Favor intente de nuevo más tarde.");
                    return View(solicitudVacaciones);
                }

                var role = roleService.ObtenerRolPorNombre("Director");
                if(role.Id == solicitante.Role.Id)
                {
                    solicitudVacaciones.SolicitudId = guid;
                    await EnviarCorreo(solicitudVacaciones, solicitante);
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
                var guid = Guid.NewGuid();
                var rowsAffected = await solicitudService.CrearSolicitudVacaciones(solicitudVacaciones, guid);

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

        [Auth(Roles = "Jefatura, Director")]
        public async Task<ActionResult> SolicitudesEmpleados(string filtro, int? pagina, string fechaInicio, string fechaFinal)
        {
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFinal = fechaFinal;

            var jefe = await userService.GetLoggedInUser();
            if (jefe == null)
                return RedirectToAction("LogOut", "Account");

            var solicitudes = solicitudService.ObtenerSolicitudesPorAprobar(jefe, fechaInicio, fechaFinal);

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
            if (usuario == null)
                return RedirectToAction("LogOut", "Account");

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
                if(usuario == null)
                    return RedirectToAction("LogOut", "Account");

                var rowsAffected = solicitudService.Aprobar(solicitud.SolicitudId, solicitud.ComentarioJefatura, usuario, solicitud.UsuarioId);
                if(rowsAffected > 0)
                {
                    await EnviarCorreo(solicitud, usuario);

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

        private async Task EnviarCorreo(SolicitudViewModel solicitudVacaciones, UserViewModel aprobador)
        {
            // Se genera el archivo PDF y se envia por correo
            var solicitudAprobada = solicitudService.ObtenerSolicitudPorId(solicitudVacaciones.SolicitudId);
            var solicitudViewModel = new SolicitudViewModel()
            {
                SolicitudId = solicitudAprobada.SolicitudVacacionesId,
                Id = solicitudAprobada.UsuarioId,
                CantidadDiasSolicitados = solicitudAprobada.CantidadDiasSolicitados,
                Categoria = solicitudAprobada.Usuario.Categoria,
                Comentario = solicitudAprobada.Comentario,
                FechaSolicitud = solicitudAprobada.FechaSolicitud,
                Nombre = solicitudAprobada.Usuario.Nombre,
                PrimerApellido = solicitudAprobada.Usuario.PrimerApellido,
                SegundoApellido = solicitudAprobada.Usuario.SegundoApellido,
                SaldoDiasDisponibles = solicitudAprobada.Usuario.SaldoDiasEmpleado.SaldoDiasDisponibles,
                UnidadTecnica = solicitudAprobada.Usuario.UnidadTecnica
            };

            var pdf = GeneratePDF(solicitudViewModel);
            await EnviarCorreoAprobacion(solicitudVacaciones.Id, aprobador, solicitudViewModel, pdf);
        }

        private async Task EnviarCorreoAprobacion(string solicitanteId, UserViewModel aprobador, SolicitudViewModel solicitud, string fileName)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var nombreSolicitante = $"{solicitante.Nombre} {solicitante.PrimerApellido} {solicitante.SegundoApellido}";
            var jefe = new UserViewModel();

            if (solicitante.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);
            if (solicitante.Role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase) || solicitante.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerDirectorGeneral();
            if (solicitante.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerUsuarioPorId(solicitanteId);
            // var jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);

            if (aprobador.Id == jefe.Id)
                await emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{ConfigurationManager.AppSettings["MailRH"]}", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitud.SolicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {solicitud.ComentarioJefatura}", Server.MapPath("~/PDFs/" + fileName));
            else
                await emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{aprobador.Email},{ConfigurationManager.AppSettings["MailRH"]}", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitud.SolicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {solicitud.ComentarioJefatura}", Server.MapPath("~/PDFs/" + fileName));
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
                if (usuario == null)
                    return RedirectToAction("LogOut", "Account");

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
                .Where(x => (string.IsNullOrEmpty(x.Comentario) ? false : x.Comentario.ToLower().Contains(filtro)) ||
                 x.UsuarioId.ToLower().Contains(filtro) ||
                 x.Estado.Nombre.ToLower().Contains(filtro) ||
                 x.Usuario.Nombre.ToLower().Contains(filtro) ||
                 x.Usuario.PrimerApellido.ToLower().Contains(filtro) ||
                 (string.IsNullOrEmpty(x.Usuario.SegundoApellido) ? false : x.Usuario.SegundoApellido.ToLower().Contains(filtro)) ||
                 x.CantidadDiasSolicitados.ToString() == filtro ||
                 x.FechaSolicitud.ToString("yyyy-MM-dd").Contains(filtro) ||
                 x.FechaSolicitud.ToString("yyyy/MM/dd").Contains(filtro) ||
                 x.FechaSolicitud.ToString("dd/MM/yyyy").Contains(filtro) ||
                 x.FechaSolicitud.ToString("dd-MM-yyyy").Contains(filtro))

                .ToList();

            return solicitudesFiltradas;
        }
    }
}
