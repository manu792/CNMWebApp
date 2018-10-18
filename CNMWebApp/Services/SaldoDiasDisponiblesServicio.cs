using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class SaldoDiasDisponiblesServicio
    {
        private ApplicationDbContext context;

        public SaldoDiasDisponiblesServicio()
        {
            context = new ApplicationDbContext();
        }

        public SaldoDiasPorEmpleado ObtenerSaldoDiasPorEmpleadoId(string id)
        {
            return context.SaldoDiasEmpleados.FirstOrDefault(x => x.EmpleadoId == id);
        }
    }
}