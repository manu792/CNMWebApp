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

        public IEnumerable<SolicitudVacaciones> ObtenerSolicitudesPorUnidadTecnica(int unidadTecnicaId)
        {
            return context.SolicitudesVacaciones
                .Where(x => x.Usuario.UnidadTecnicaId == unidadTecnicaId)
                .ToList();
        }
    }
}