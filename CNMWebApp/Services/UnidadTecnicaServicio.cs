using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class UnidadTecnicaServicio
    {
        private ApplicationDbContext context;

        public UnidadTecnicaServicio()
        {
            context = new ApplicationDbContext();
        }

        public IEnumerable<UnidadTecnica> ObtenerUnidadesTecnicas()
        {
            return context.UnidadesTecnicas
                .Where(x => !x.Nombre.ToLower().Equals("manager"));
        }
    }
}