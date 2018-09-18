using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNMWebApp.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string Nombre { get; set; }
        [DisplayName("Primer Apellido")]
        [Required(ErrorMessage = "El campo Primer Apellido es requerido")]
        public string PrimerApellido { get; set; }
        [DisplayName("Segundo Apellido")]
        public string SegundoApellido { get; set; }
        [DisplayName("Correo")]
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Celular")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DisplayName("Fecha Ingreso")]
        [Required(ErrorMessage = "El campo Fecha Ingreso es requerido")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }
    }

    public class UserRolesViewModel : UserViewModel
    {
        // Roles
        [Required]
        public string SelectedRoleId { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public IdentityRole Role { get; set; }

        // Categorias
        public string SelectedCategoriaId { get; set; }
        public List<Categoria> Categorias { get; set; }
        public Categoria Categoria { get; set; }

        // Unidades Tecnicas
        public string SelectedUnidadTecnicaId { get; set; }
        public List<UnidadTecnica> UnidadesTecnicas { get; set; }
        public UnidadTecnica UnidadTecnica { get; set; }

        // Jefes
        public string SelectedJefeId { get; set; }
        public List<UserViewModel> Jefes { get; set; }
        public UserViewModel Jefe { get; set; }
    }
}