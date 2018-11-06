using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNMWebApp.Models
{
    public class UserViewModel
    {
        [DisplayName("Cédula")]
        [Required(ErrorMessage = "El campo Cédula es requerido")]
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
        //[DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }
        [DisplayName("Fecha Ingreso")]
        [Required(ErrorMessage = "El campo Fecha Ingreso es requerido")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaIngreso { get; set; }
        [DisplayName("Fecha Creación Empleado")]
        [Required(ErrorMessage = "El campo Fecha Creación Empleado es requerido")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }
        public HttpPostedFileWrapper Foto { get; set; }
        [DisplayName("Foto")]
        public string FotoRuta { get; set; }
        [DisplayName("Activo")]
        public bool EstaActivo { get; set; }
        [DisplayName("Superusuario")]
        public bool EsSuperusuario { get; set; }
        [DisplayName("Saldo Días Disponibles")]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$", ErrorMessage = "Debe ser un valor numérico")]
        [Range(0, 1000, ErrorMessage = "Debe ser un número entre 0 y 1000")]
        [Required(ErrorMessage = "El campo Saldo Días Disponibles es requerido")]
        [DefaultValue(0)]
        //[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal SaldoDiasDisponibles { get; set; }
        [DisplayName("Jefe")]
        public string JefeId { get; set; }

        // Vinculan el usuario con los datos de las tablas Roles, Categoria y UnidadTecnica
        [DisplayName("Rol")]
        public IdentityRole Role { get; set; }
        [DisplayName("Categoría")]
        public Categoria Categoria { get; set; }
        [DisplayName("Unidad Técnica")]
        public UnidadTecnica UnidadTecnica { get; set; }
        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {PrimerApellido}";
            }
        }
    }

    public class UserRolesViewModel : UserViewModel
    {
        // Roles
        [Required(ErrorMessage = "El campo Rol es requerido")]
        public string SelectedRoleId { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    public class UserRolesUnidadCategoria : UserRolesViewModel
    {
        // Categorias
        [Required(ErrorMessage = "El campo Categoría es requerido")]
        public string SelectedCategoriaId { get; set; }
        public List<Categoria> Categorias { get; set; }

        // Unidades Tecnicas
        [Required(ErrorMessage = "El campo Unidad Técnica es requerido")]
        public string SelectedUnidadTecnicaId { get; set; }
        public List<UnidadTecnica> UnidadesTecnicas { get; set; }


        // Jefes
        [Required(ErrorMessage = "El campo Jefe es requerido")]
        public string SelectedJefeId { get; set; }
        public List<UserViewModel> Jefes { get; set; }
    }

    public class SolicitudViewModel : UserViewModel
    {
        public Guid SolicitudId { get; set; }
        [DisplayName("Cédula")]
        public string UsuarioId { get; set; }
        [DisplayName("Días a solicitar")]
        // [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar al menos un día para solicitar sus vacaciones")]
        public string Dias { get; set; }
        [DisplayName("Años Laborados")]
        public int CantidadAnnosLaborados { get; set; }
        [DisplayName("Cantidad Días Solicitados")]
        public int CantidadDiasSolicitados { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "El comentario debe ser máximo de 500 caracteres")]
        public string Comentario { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "El comentario debe ser máximo de 500 caracteres")]
        [DisplayName("Observaciones")]
        public string ComentarioJefatura { get; set; }
        public IEnumerable<DiasPorSolicitudViewModel> DiasPorSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
    }

    public class SolicitudParaEmpleado : SolicitudViewModel
    {
        [Required(ErrorMessage = "El campo Colaborador es requerido")]
        public string ColaboradorId { get; set; }
        public List<UserViewModel> Colaboradores { get; set; }
        [DisplayName("Fecha Ingreso")]
        public string FechaIngresoEmpleado { get; set; }
        [DisplayName("Fecha Creación Empleado")]
        public string FechaCreacionUsuario { get; set; }
    }

    public class DiasPorSolicitudViewModel
    {
        public string UsuarioId { get; set; }
        public string Fecha { get; set; }
        public string Periodo { get; set; }
    }
}