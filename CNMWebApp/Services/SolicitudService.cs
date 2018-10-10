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

        public SolicitudService()
        {
            context = new ApplicationDbContext();
            userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userService = new UserService();
        }

        public IEnumerable<SolicitudVacaciones> ObtenerMisSolicitudes(UserViewModel usuario)
        {
            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.Id == usuario.Id)
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

        public async Task<int> CrearSolicitudVacaciones(VacacionViewModel solicitud)
        {
            var solicitudVacaciones = new SolicitudVacaciones()
            {
                UsuarioId = solicitud.Id,
                CantidadDiasSolicitados = solicitud.CantidadDiasSolicitados,
                EstadoId = 1,
                Comentario = solicitud.Comentario,
                FechaSolicitud = DateTime.Now,
                DiasPorSolicitud = ObtenerDiasPorSolicitud(solicitud)
            };

            // A continuacion se envia la notificacion por correo al jefe correspondiente segun el usuario
            var callbackUrl = @"Controller\Action?parameter1=something";
            await userManager.SendEmailAsync(ObtenerAprobadorId(solicitud.Id), $"Solicitud de Vacaciones para {solicitud.Nombre} {solicitud.PrimerApellido} {solicitud.SegundoApellido}", $"Para aprobar o rechazar la solicitud de vacaciones haga click en el siguiente link: <a href=\\ {callbackUrl} \\>here</a>");


            context.SolicitudesVacaciones.Add(solicitudVacaciones);
            return context.SaveChanges();
        }

        private string ObtenerAprobadorId(string id)
        {
            var empleado = userService.ObtenerUsuarioPorId(id);
            var aprobador = new UserViewModel();

            if (empleado.Role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
            {
                // La solicitud se envia al jefe directo
                aprobador = userService.ObtenerJefe(empleado.UnidadTecnica.UnidadTecnicaId);
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

        private ICollection<DiasPorSolicitud> ObtenerDiasPorSolicitud(VacacionViewModel solicitud)
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