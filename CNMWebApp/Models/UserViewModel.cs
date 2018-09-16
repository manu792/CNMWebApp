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
        [DisplayName("Usuario")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Correo")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Celular")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }

    public class UserRolesViewModel : UserViewModel
    {
        // Available Roles for selecting
        [Required]
        public string SelectedRoleId { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public IdentityRole Role { get; set; }
    }
}