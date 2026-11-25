using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Comun;

namespace ClinicaDental.Contrato.DTOs.Auth
{
    /// <summary>Cuerpo de POST /api/Auth/usuarios (solo Administrador).</summary>
    public class RegistrarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 30 caracteres.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z0-9._-]+$", ErrorMessage = "Debe contener al menos una letra; solo letras, números, punto, guion y guion bajo (sin espacios).")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre completo debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^\p{L}+([ '\-]\p{L}+)*$", ErrorMessage = "El nombre completo solo puede contener letras, espacios, apóstrofe o guion.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [ContrasenaSegura]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma la contraseña.")]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = Roles.Recepcionista;
    }
}
