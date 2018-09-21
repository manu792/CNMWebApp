﻿using Microsoft.AspNet.Identity.EntityFramework;
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
        [DisplayName("Cedula")]
        [Required(ErrorMessage = "El campo Cedula es requerido")]
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
        public HttpPostedFileWrapper Foto { get; set; }
        [DisplayName("Foto")]
        public string FotoRuta { get; set; }
        [DisplayName("Activo")]
        public bool EstaActivo { get; set; }

        // Vinculan el usuario con los datos de las tablas Roles, Categoria y UnidadTecnica
        [DisplayName("Rol")]
        public IdentityRole Role { get; set; }
        public Categoria Categoria { get; set; }
        [DisplayName("Unidad Tecnica")]
        public UnidadTecnica UnidadTecnica { get; set; }
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
        [Required(ErrorMessage = "El campo Categoria es requerido")]
        public string SelectedCategoriaId { get; set; }
        public List<Categoria> Categorias { get; set; }

        // Unidades Tecnicas
        [Required(ErrorMessage = "El campo Unidad Tecnica es requerido")]
        public string SelectedUnidadTecnicaId { get; set; }
        public List<UnidadTecnica> UnidadesTecnicas { get; set; }
    }
}