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
        private RoleService _roleServicio;

        public UnidadTecnicaServicio()
        {
            context = new ApplicationDbContext();
            _roleServicio = new RoleService();
        }

        public IEnumerable<UnidadTecnica> ObtenerUnidadesTecnicas()
        {
            return context.UnidadesTecnicas
                .Where(x => !x.Nombre.ToLower()
                  .Equals("todas las unidades técnicas", StringComparison.OrdinalIgnoreCase));
        }

        public UnidadTecnica ObtenerUnidadTecnicaPorId(int id)
        {
            return context.UnidadesTecnicas
                .FirstOrDefault(x => x.UnidadTecnicaId == id);
        }

        public IEnumerable<UnidadTecnica> ObtenerUnidadesTecnicasPorRoleId(string roleId)
        {
            var unidades = context.UnidadesTecnicas.ToList();

            var role =_roleServicio.ObtenerRolPorId(roleId);
            if(!role.Name.Equals("jefatura", StringComparison.OrdinalIgnoreCase) && !role.Name.Equals("funcionario", StringComparison.OrdinalIgnoreCase))
            {
                unidades = unidades.Where(x => x.Nombre
                    .Equals("todas las unidades técnicas", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return unidades;
            }

            return unidades.Where(x => !x.Nombre.Equals("todas las unidades técnicas", StringComparison.OrdinalIgnoreCase));
        }
    }
}