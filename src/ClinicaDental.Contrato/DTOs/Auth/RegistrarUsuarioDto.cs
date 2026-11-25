using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Comun;

namespace ClinicaDental.Contrato.DTOs.Auth
{
    /// <summary>Cuerpo de POST /api/Auth/usuarios (solo Administrador).</summary>
    public class RegistrarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MinLength(4, ErrorMessage = "El nombre de usuario debe tener al menos 4 caracteres.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Solo letras, números, punto, guion y guion bajo (sin espacios).")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [MaxLength(120, ErrorMessage = "El nombre completo no puede tener más de 120 caracteres.")]
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
