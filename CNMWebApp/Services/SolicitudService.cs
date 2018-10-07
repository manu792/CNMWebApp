using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class SolicitudService
    {
        private ApplicationDbContext context;

        public SolicitudService()
        {
            context = new ApplicationDbContext();
        }

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudes(UserViewModel usuario, bool misSolicitudes)
        {
            switch (usuario.Role.Name)
            {
                case "Funcionario":
                    return ObtenerSolicitudesParaFuncionario(usuario.Id);
                case "Jefatura":
                    if(misSolicitudes)
                        return ObtenerSolicitudesParaFuncionario(usuario.Id);
                    else
                        return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
                case "Director":
                    if(misSolicitudes)
                        return ObtenerSolicitudesParaFuncionario(usuario.Id);
                    else
                        return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
                case "Recursos Humanos":
                    if (misSolicitudes)
                        return ObtenerSolicitudesParaFuncionario(usuario.Id);
                    else
                        return ObtenerSolicitudesPorUnidadTecnica(usuario.UnidadTecnica.UnidadTecnicaId);
            }

            return new List<SolicitudVacaciones>();
        }

        private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorUnidadTecnica(int unidadTecnicaId)
        {
            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.UnidadTecnicaId == unidadTecnicaId)
                .ToList();
        }

        private IEnumerable<SolicitudVacaciones> ObtenerSolicitudesParaFuncionario(string id)
        {
            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.Id == id)
                .ToList();
        }

        public int CrearSolicitudVacaciones(VacacionViewModel solicitud)
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
            

            context.SolicitudesVacaciones.Add(solicitudVacaciones);
            return context.SaveChanges();
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
                        Fecha = diaFecha
                    });
                else
                    throw new Exception("Formato de fecha inválido");
            }

            return diasPorSolicitud;
        }
    }
}