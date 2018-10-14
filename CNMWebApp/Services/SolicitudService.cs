using CNMWebApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CNMWebApp.Services
{
    public class SolicitudService
    {
        private ApplicationDbContext context;
        private ApplicationUserManager userManager;
        private UserService userService;
        private EmailNotificationService emailNotification;

        public SolicitudService()
        {
            context = new ApplicationDbContext();
            userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userService = new UserService();
            emailNotification = new EmailNotificationService();
        }

        public IEnumerable<SolicitudVacaciones> ObtenerMisSolicitudes(UserViewModel usuario)
        {
            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.Id == usuario.Id)
                .ToList();
        }

        public SolicitudVacaciones ObtenerSolicitudPorId(int id)
        {
            return context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == id);
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

            var solicitudVacaciones = new SolicitudVacaciones()
            {
                UsuarioId = solicitud.Id,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                EstadoId = 1,
                Comentario = solicitud.Comentario,
                FechaSolicitud = DateTime.Now,
                AprobadorId = solicitante.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase) ?
                     userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId).Id :
                    userService.ObtenerDirectorGeneral().Id,
                DiasPorSolicitud = ObtenerDiasPorSolicitud(solicitud)
            };

            // A continuacion se envia la notificacion por correo al jefe correspondiente segun el usuario
            var callbackUrl = @"Controller\Action?parameter1=something";
            await userManager.SendEmailAsync(ObtenerAprobadorId(solicitud.Id), $"Solicitud de Vacaciones para {solicitud.Nombre} {solicitud.PrimerApellido} {solicitud.SegundoApellido}", $"{solicitud.Comentario} <br /> Para aprobar o rechazar la solicitud de vacaciones haga click en el siguiente link: <a href=\\ {callbackUrl} \\>here</a>");


            context.SolicitudesVacaciones.Add(solicitudVacaciones);
            return context.SaveChanges();
        }

        public int Aprobar(int solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);

            var nombreSolicitante = $"{solicitante.Nombre} {solicitante.PrimerApellido} {solicitante.SegundoApellido}";

            var estadoAprobado = context.Estados.FirstOrDefault(x => x.Nombre.Equals("aprobado", StringComparison.OrdinalIgnoreCase));
            var solicitud = context.SolicitudesVacaciones.FirstOrDefault(x => x.SolicitudVacacionesId == solicitudId);

            solicitud.EstadoId = estadoAprobado.EstadoId;

            context.SolicitudesVacaciones.Add(solicitud);
            context.Entry(solicitud).State = System.Data.Entity.EntityState.Modified;
            var rowsAffected = context.SaveChanges();

            if(rowsAffected > 0)
            {
                // Envio correo de aprobacion al solicitante con copia a RH y al aprobador, asi como al jefe del solicitante. 
                // Se adjunta boleta en formato PDF
                // Verificar si el que aprobo las vacaciones es el jefe o el director, y enviar el correo a ambos si es necesario
                if(aprobador.Id == jefe.Id)
                    emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},otistestuh@gmail.com", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {comentarioJefatura}");
                else
                    emailNotification.SendEmailAsync(solicitante.Email, $"{jefe.Email},{aprobador.Email},otistestuh@gmail.com", $"Vacaciones Aprobadas para {nombreSolicitante}", $"La solicitud de vacaciones: {solicitudId} para el colaborador {nombreSolicitante} fue <strong>aprobada</strong>. <br /> <br /> Observaciones: {comentarioJefatura}");
            }

            return rowsAffected;
        }

        public int Rechazar(int solicitudId, string comentarioJefatura, UserViewModel aprobador, string solicitanteId)
        {
            var solicitante = userService.ObtenerUsuarioPorId(solicitanteId);
            var jefe = userService.ObtenerJefePorUnidadTecnica(solicitante.UnidadTecnica.UnidadTecnicaId);

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
    }
}