using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class EstadoServicio
    {
        private ApplicationDbContext context;

        public EstadoServicio()
        {
            context = new ApplicationDbContext();
        }

        public Estado ObtenerEstadoPorNombre(string nombre)
        {
            return context.Estados.FirstOrDefault(x => x.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}