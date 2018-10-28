using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNMWebApp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El formato es incorrecto")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El formato es incorrecto")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido")]
        [StringLength(100, ErrorMessage = "El campo Contraseña debe ser de al menos {2} caracteres de longitud", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación deben ser iguales")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido")]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación deben ser iguales")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El campo Correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto")]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
