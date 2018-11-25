using CNMWebApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Services
{
    public class SolicitudService
    {
        private ApplicationDbContext context;
        private ApplicationUserManager userManager;
        private UserService userService;
        private SaldoDiasDisponiblesServicio saldoDiasService;
        private EmailNotificationService emailNotification;
        private RoleService roleService;
        private CategoriaServicio categoriaService;

        public SolicitudService()
        {
            context = new ApplicationDbContext();
            userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userService = new UserService();
            saldoDiasService = new SaldoDiasDisponiblesServicio();
            emailNotification = new EmailNotificationService();
            roleService = new RoleService();
            categoriaService = new CategoriaServicio();
        }

        public IEnumerable<SolicitudVacaciones> ObtenerMisSolicitudes(UserViewModel usuario, string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            if(!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if(DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones
                        .Where(x => x.Usuario.Id == usuario.Id && x.FechaSolicitud >= fechaI && x.FechaSolicitud <= fechaF)
                        .ToList();
                }
            }

            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.Id == usuario.Id)
                .ToList();
        }

        public SolicitudVacaciones ObtenerSolicitudPorId(int id)
        {
            return context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == id);
        }

        public IEnumerable<SolicitudVacaciones> ObtenerTodasSolicitudesPorRol(string fechaInicio, string fechaFinal, UserViewModel usuario)
        {
            DateTime fechaI;
            DateTime fechaF;

            if(usuario.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase) && usuario.EsSuperusuario)
            {
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                {
                    if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                    {
                        return context.SolicitudesVacaciones.Where(x => x.FechaSolicitud >= fechaI &&
                            x.FechaSolicitud <= fechaF);
                    }
                }

                return context.SolicitudesVacaciones;
            }

            else if(usuario.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                {
                    if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                    {
                        return context.SolicitudesVacaciones.Where(x => x.FechaSolicitud >= fechaI &&
                            x.FechaSolicitud <= fechaF &&
                            x.UsuarioId == usuario.Id);
                    }
                }

                return context.SolicitudesVacaciones.Where(x => x.UsuarioId == usuario.Id);
            }

            else if (usuario.Role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                {
                    if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                    {
                        return context.SolicitudesVacaciones.Where(x => x.FechaSolicitud >= fechaI &&
                            x.FechaSolicitud <= fechaF &&
                            x.Usuario.UnidadTecnicaId == usuario.UnidadTecnica.UnidadTecnicaId);
                    }
                }

                return context.SolicitudesVacaciones.Where(x => x.Usuario.UnidadTecnicaId == usuario.UnidadTecnica.UnidadTecnicaId);
            }

            else if (usuario.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase) || 
                     usuario.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase) || 
                     usuario.Role.Name.Equals("manager", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                {
                    if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                    {
                        return context.SolicitudesVacaciones.Where(x => x.FechaSolicitud >= fechaI &&
                            x.FechaSolicitud <= fechaF);
                    }
                }

                return context.SolicitudesVacaciones;
            }
            else
            {
                throw new Exception("Rol inválido");
            }
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesFuncionarios(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Funcionario");

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) && 
                        x.FechaSolicitud >= fechaI && 
                        x.FechaSolicitud <= fechaF);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id));
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesFuncionariosPorUnidad(string fechaInicio, string fechaFinal, UserViewModel usuario)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Funcionario");

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                        x.FechaSolicitud >= fechaI &&
                        x.FechaSolicitud <= fechaF &&
                        x.Usuario.UnidadTecnicaId == usuario.UnidadTecnica.UnidadTecnicaId);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                x.Usuario.UnidadTecnicaId == usuario.UnidadTecnica.UnidadTecnicaId);
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesJefaturas(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Jefatura");

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                        x.FechaSolicitud >= fechaI &&
                        x.FechaSolicitud <= fechaF);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id));
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesRH(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Recursos Humanos");

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                        x.FechaSolicitud >= fechaI &&
                        x.FechaSolicitud <= fechaF);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id));
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesDirectorGeneral(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Director");
            var categoria = categoriaService.ObtenerCategoriaPorNombre("Director General");

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                        x.Usuario.CategoriaId == categoria.CategoriaId &&
                        x.FechaSolicitud >= fechaI &&
                        x.FechaSolicitud <= fechaF);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) && 
                x.Usuario.CategoriaId == categoria.CategoriaId);
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesDirectorAdministrativo(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Director");
            var categoria = categoriaService.ObtenerCategoriaPorNombre("Director Administrativo");


            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                        x.Usuario.CategoriaId == categoria.CategoriaId &&
                        x.FechaSolicitud >= fechaI &&
                        x.FechaSolicitud <= fechaF);
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Usuario.Roles.Any(r => r.RoleId == role.Id) &&
                x.Usuario.CategoriaId == categoria.CategoriaId);
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorAprobar(UserViewModel jefe, string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                if (DateTime.TryParse(fechaInicio, out fechaI) && DateTime.TryParse(fechaFinal, out fechaF))
                {
                    return context.SolicitudesVacaciones.Where(x => x.Estado.Nombre.Equals("por revisar", StringComparison.OrdinalIgnoreCase) &&
                       x.AprobadorId == jefe.Id &&
                       x.UsuarioId != jefe.Id &&
                       x.FechaSolicitud >= fechaI &&
                       x.FechaSolicitud <= fechaF)
                    .ToList();
                }
            }

            return context.SolicitudesVacaciones.Where(x => x.Estado.Nombre.Equals("por revisar", StringComparison.OrdinalIgnoreCase) &&
                   x.AprobadorId == jefe.Id &&
                   x.UsuarioId != jefe.Id)
            .ToList();
        }

        public async Task<int> CrearSolicitudVacaciones(SolicitudViewModel solicitud)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitud.Id);
            var fechaSolicitud = DateTime.Now;

            var aprobadorId = ObtenerAprobadorId(solicitud.Id);

            var solicitudVacaciones = new SolicitudVacaciones()
            {
                UsuarioId = solicitud.Id,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                EstadoId = 1,
                Comentario = solicitud.Comentario,
                FechaSolicitud = fechaSolicitud,
                UltimaActualizacion = fechaSolicitud,
                AprobadorId = aprobadorId,
                DiasPorSolicitud = ObtenerDiasPorSolicitud(solicitud, fechaSolicitud)
            };

            context.SolicitudesVacaciones.Add(solicitudVacaciones);
            var affectedRows = context.SaveChanges();

            if(affectedRows > 0)
            {
                // A continuacion se envia la notificacion por correo al jefe correspondiente segun el usuario
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var callbackUrl = urlHelper.Action("Revisar", "Solicitud", new { id = solicitudVacaciones.SolicitudVacacionesId }, protocol: HttpContext.Current.Request.Url.Scheme);
                
                if (aprobadorId == solicitud.Id)
                    return ProcesarSolicitud(solicitudVacaciones.SolicitudVacacionesId, solicitante);

                await userManager.SendEmailAsync(ObtenerAprobadorId(solicitud.Id), "Solicitud de Vacaciones para " + solicitud.Nombre + " " + solicitud.PrimerApellido + " " + solicitud.SegundoApellido, solicitud.Comentario + " <br /> Para aprobar o rechazar la solicitud de vacaciones haga click en el siguiente link: <a href=\"" + callbackUrl + "\">Aquí</a>");
            }

            return solicitudVacaciones.SolicitudVacacionesId;
        }

        public int Aprobar(int solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var saldoDiasInfo = saldoDiasService.ObtenerSaldoDiasPorEmpleadoId(solicitanteId);

            var nombreSolicitante = $"{solicitante.Nombre} {solicitante.PrimerApellido} {solicitante.SegundoApellido}";

            var estadoAprobado = context.Estados.FirstOrDefault(x => x.Nombre.Equals("aprobado", StringComparison.OrdinalIgnoreCase));
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == solicitudId);
            solicitud.Usuario = context.Users.FirstOrDefault(x => x.Id == solicitud.UsuarioId);

            // Se actualiza el estado de la solicitud a Aprobada
            // Se deducen los dias solicitados a la cantidad de saldo de dias disponibles
            solicitud.EstadoId = estadoAprobado.EstadoId;
            solicitud.Usuario.SaldoDiasEmpleado.SaldoDiasDisponibles -= solicitud.CantidadDiasSolicitados;
            solicitud.Usuario.SaldoDiasEmpleado.UltimaActualizacion = DateTime.Now;
            context.SolicitudesVacaciones.Add(solicitud);
            context.Entry(solicitud).State = System.Data.Entity.EntityState.Modified;

            var rowsAffected = context.SaveChanges();
            if (rowsAffected <= 0)
                return 0;

            return solicitud.SolicitudVacacionesId;
        }

        public int Rechazar(int solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var jefe = new UserViewModel();

            if (solicitante.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerJefePorEmpleadoId(solicitante.JefeId);
            if (solicitante.Role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase) || solicitante.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerDirectorGeneral();
            if (solicitante.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerUsuarioPorId(solicitanteId);

            var nombreSolicitante = $"{solicitante.Nombre} {solicitante.PrimerApellido} {solicitante.SegundoApellido}";

            var estadoRechazado = context.Estados.FirstOrDefault(x => x.Nombre.Equals("rechazado", StringComparison.OrdinalIgnoreCase));
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == solicitudId);

            solicitud.EstadoId = estadoRechazado.EstadoId;

            context.SolicitudesVacaciones.Add(solicitud);
            context.Entry(solicitud).State = System.Data.Entity.EntityState.Modified;
            var rowsAffected = context.SaveChanges();

            if (rowsAffected > 0)
            {
                if (aprobador.Id == jefe.Id)
                    emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email}", $"Vacaciones Denegadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>denegada</strong>. <br /> Observaciones: {comentarioJefatura}");
                else
                    emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{aprobador.Email}", $"Vacaciones Denegadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>denegada</strong>. <br /> Observaciones: {comentarioJefatura}");
            }

            return solicitud.SolicitudVacacionesId;
        }

        public int ObtenerCantidadDiasDisfrutadosPeriodoPorEmpleado(string empleadoId)
        {
            var currentYear = DateTime.Now.Year;

            var solicitudes = context.SolicitudesVacaciones
                .Where(x => x.FechaSolicitud.Year == currentYear && 
                x.UsuarioId == empleadoId && 
                x.Estado.Nombre.Equals("aprobado", StringComparison.OrdinalIgnoreCase));

            var dias = solicitudes.Sum(x => x.CantidadDiasSolicitados);
                
            return dias;
        }

        private string ObtenerAprobadorId(string id)
        {
            var empleado = userService.ObtenerUsuarioPorId(id);
            var aprobador = new UserViewModel();

            if (empleado.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
            {
                // La solicitud se envia al jefe directo
                aprobador = userService.ObtenerJefePorEmpleadoId(empleado.JefeId);
            }
            else if (empleado.Role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase) || empleado.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase))
            {
                // La solicitud se envia al director general
                aprobador = userService.ObtenerDirectorGeneral();
            }
            else if (empleado.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase))
            {
                // La solicitud se aprueba automaticamente en caso de tener dias disponibles
                aprobador = userService.ObtenerUsuarioPorId(id);
            }
            else
            {
                throw new Exception("El rol no puede realizar solicitudes de vacaciones");
            }

            return aprobador.Id;
        }

        private ICollection<DiasPorSolicitud> ObtenerDiasPorSolicitud(SolicitudViewModel solicitud, DateTime fechaSolicitud)
        {
            var dias = solicitud.Dias.Split(',');
            DateTime diaFecha;
            var diasPorSolicitud = new List<DiasPorSolicitud>();

            foreach (var dia in dias)
            {
                if (DateTime.TryParse(dia, out diaFecha))
                    diasPorSolicitud.Add(new DiasPorSolicitud()
                    {
                        UsuarioId = solicitud.Id,
                        Fecha = diaFecha,
                        Periodo = $"{fechaSolicitud.Year}-{fechaSolicitud.Year + 1}"
                    });
                else
                    throw new Exception("Formato de fecha inválido");
            }

            return diasPorSolicitud;
        }

        private int ProcesarSolicitud(int guid, UserViewModel solicitante)
        {
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == guid);
            if (solicitud == null)
                throw new Exception("Id solicitud no encontrado.");

            if (solicitud.CantidadDiasSolicitados <= solicitante.SaldoDiasDisponibles)
                return Aprobar(guid, "", solicitante, solicitante.Id);
            else
                return Rechazar(guid, "", solicitante, solicitante.Id);

        }
    }
}