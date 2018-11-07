using CNMWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNMWebApp.Services
{
    public class CategoriaServicio
    {
        private ApplicationDbContext context;

        public CategoriaServicio()
        {
            context = new ApplicationDbContext();
        }

        public IEnumerable<Categoria> ObtenerCategorias()
        {
            return context.Categorias
                .Where(x => !x.Nombre.ToLower().Equals("manager"));
        }

        public Categoria ObtenerCategoriaPorId(int id)
        {
            return context.Categorias.FirstOrDefault(x => x.CategoriaId == id);
        }

        public IEnumerable<Categoria> ObtenerCategoriasPorRoleId(string roleId)
        {
            return context.Categorias
                .Where(x => x.RolId == roleId);
        }

        public Categoria ObtenerCategoriaPorNombre(string nombre)
        {
            return context.Categorias
                .Where(x => x.Nombre.ToLower() == nombre.ToLower())
                .FirstOrDefault();
        }
    }
}