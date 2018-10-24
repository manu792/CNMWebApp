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

        public SolicitudVacaciones ObtenerSolicitudPorId(Guid id)
        {
            return context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == id);
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudes(string fechaInicio, string fechaFinal)
        {
            DateTime fechaI;
            DateTime fechaF;

            var role = roleService.ObtenerRolPorNombre("Funcionario");

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

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorAprobar(UserViewModel jefe)
        {
            return context.SolicitudesVacaciones.Where(x => x.Estado.Nombre.Equals("por revisar", StringComparison.OrdinalIgnoreCase) &&
                   x.AprobadorId == jefe.Id &&
                   x.UsuarioId != jefe.Id)
            .ToList();
        }

        //public IEnumerable<SolicitudVacaciones> ObtenerSolicitudes(UserViewModel usuario, bool misSolicitudes)
        //{
        //    switch (usuario.Role.Name)
        //    {
        //        case "Funcionario":
        //            if(misSolicitudes)
        //                return ObtenerSolicitudesParaFuncionario(usuario.Id);
        //            else
        //                return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
        //        case "Jefatura":
        //            if(misSolicitudes)
        //                return ObtenerSolicitudesParaFuncionario(usuario.Id);
        //            else
        //                return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
        //        case "Director":
        //            if(misSolicitudes)
        //                return ObtenerSolicitudesParaFuncionario(usuario.Id);
        //            else
        //                return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
        //        case "Recursos Humanos":
        //            if (misSolicitudes)
        //                return ObtenerSolicitudesParaFuncionario(usuario.Id);
        //            else
        //                return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
        //    }

        //    return new List<SolicitudVacaciones>();
        //}

        //private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorUnidadTecnica(int unidadTecnicaId)
        //{
        //    return context.SolicitudesVacaciones
        //        .Where(x => x.Usuario.UnidadTecnicaId == unidadTecnicaId)
        //        .ToList();
        //}

        //private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesParaFuncionario(string id)
        //{
        //    return context.SolicitudesVacaciones
        //        .Where(x => x.Usuario.Id == id)
        //        .ToList();
        //}

        public async Task<int> CrearSolicitudVacaciones(SolicitudViewModel solicitud)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitud.Id);
            var fechaSolicitud = DateTime.Now;
            var guid = Guid.NewGuid();

            var aprobadorId = ObtenerAprobadorId(solicitud.Id);

            var solicitudVacaciones = new SolicitudVacaciones()
            {
                SolicitudVacacionesId = guid,
                UsuarioId = solicitud.Id,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                EstadoId = 1,
                Comentario = solicitud.Comentario,
                FechaSolicitud = fechaSolicitud,
                AprobadorId = aprobadorId,
                DiasPorSolicitud = ObtenerDiasPorSolicitud(solicitud)
            };

            context.SolicitudesVacaciones.Add(solicitudVacaciones);
            var affectedRows = context.SaveChanges();

            if(affectedRows > 0)
            {
                // A continuacion se envia la notificacion por correo al jefe correspondiente segun el usuario
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var callbackUrl = urlHelper.Action("Revisar", "Solicitud", new { id = guid }, protocol: HttpContext.Current.Request.Url.Scheme);
                
                if (aprobadorId == solicitud.Id)
                    return await ProcesarSolicitud(guid, solicitante);

                await userManager.SendEmailAsync(ObtenerAprobadorId(solicitud.Id), "Solicitud de Vacaciones para " + solicitud.Nombre + " " + solicitud.PrimerApellido + " " + solicitud.SegundoApellido, solicitud.Comentario + " <br /> Para aprobar o rechazar la solicitud de vacaciones haga click en el siguiente link: <a href=\"" + callbackUrl + "\">Aquí</a>");
            }
            
            return affectedRows;
        }

        public async Task<int> Aprobar(Guid solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var saldoDiasInfo = saldoDiasService.ObtenerSaldoDiasPorEmpleadoId(solicitanteId);
            var jefe = new UserViewModel();

            if(solicitante.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);
            if (solicitante.Role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase) || solicitante.Role.Name.Equals("recursos humanos", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerDirectorGeneral();
            if (solicitante.Role.Name.Equals("director", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerUsuarioPorId(solicitanteId);
            // var jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);

            var nombreSolicitante = $"{solicitante.Nombre} {solicitante.PrimerApellido} {solicitante.SegundoApellido}";

            var estadoAprobado = context.Estados.FirstOrDefault(x => x.Nombre.Equals("aprobado", StringComparison.OrdinalIgnoreCase));
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == solicitudId);

            // Se actualiza el estado de la solicitud a Aprobada
            solicitud.EstadoId = estadoAprobado.EstadoId;
            context.SolicitudesVacaciones.Add(solicitud);
            context.Entry(solicitud).State = System.Data.Entity.EntityState.Modified;
            
            // Se deducen los dias solicitados a la cantidad de saldo de dias disponibles
            saldoDiasInfo.SaldoDiasDisponibles -= solicitud.CantidadDiasSolicitados;
            saldoDiasInfo.UltimaActualizacion = DateTime.Now;
            context.SaldoDiasEmpleados.Add(saldoDiasInfo);
            context.Entry(saldoDiasInfo).State = System.Data.Entity.EntityState.Modified;

            var rowsAffected = context.SaveChanges();

            if (rowsAffected > 0)
            {
                // Envio correo de aprobacion al solicitante con copia a RH y al aprobador, asi como al jefe del solicitante. 
                // Se adjunta boleta en formato PDF
                // Verificar si el que aprobo las vacaciones es el jefe o el director, y enviar el correo a ambos si es necesario
                if (aprobador.Id == jefe.Id)
                    await emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{ConfigurationManager.AppSettings["MailRH"]}", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {comentarioJefatura}");
                else
                    await emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{aprobador.Email},{ConfigurationManager.AppSettings["MailRH"]}", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {comentarioJefatura}");
            }

            return rowsAffected;
        }

        public int Rechazar(Guid solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var jefe = new UserViewModel();

            if (solicitante.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
                jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);
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

            return rowsAffected;
        }

        private string ObtenerAprobadorId(string id)
        {
            var empleado = userService.ObtenerUsuarioPorId(id);
            var aprobador = new UserViewModel();

            if (empleado.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
            {
                // La solicitud se envia al jefe directo
                aprobador = userService.ObtenerJefePorUnidadTecnica(empleado.UnidadTecnica.UnidadTecnicaId);
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

        private ICollection<DiasPorSolicitud> ObtenerDiasPorSolicitud(SolicitudViewModel solicitud)
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
                        Periodo = $"{diaFecha.Year}-{diaFecha.Year + 1}"
                    });
                else
                    throw new Exception("Formato de fecha inválido");
            }

            return diasPorSolicitud;
        }

        private async Task<int> ProcesarSolicitud(Guid guid, UserViewModel solicitante)
        {
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == guid);
            if (solicitud == null)
                throw new Exception("Id solicitud no encontrado.");

            if (solicitud.CantidadDiasSolicitados <= solicitante.SaldoDiasDisponibles)
                return await Aprobar(guid, "", solicitante, solicitante.Id);
            else
                return Rechazar(guid, "", solicitante, solicitante.Id);

        }
    }
}