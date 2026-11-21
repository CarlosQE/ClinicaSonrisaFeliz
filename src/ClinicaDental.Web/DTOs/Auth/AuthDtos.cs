using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
        public string Contrasena { get; set; } = string.Empty;
    }

    /// <summary>Respuesta de POST /api/Auth/login.</summary>
    public class LoginRespuestaDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }

    public class RegistrarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MinLength(4, ErrorMessage = "El nombre de usuario debe tener al menos 4 caracteres.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El nombre de usuario solo puede tener letras, números, punto, guion y guion bajo.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [MaxLength(120, ErrorMessage = "El nombre completo no puede tener más de 120 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; } = string.Empty;
    }

    public class CambiarContrasenaDto
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
        public string ContrasenaNueva { get; set; } = string.Empty;
    }

    public class CambiarEstadoUsuarioDto
    {
        [Required(ErrorMessage = "Debe indicar si el usuario queda activo o inactivo.")]
        public bool? Activo { get; set; }
    }

    /// <summary>Datos públicos de un usuario (sin hash de contraseña).</summary>
    public class UsuarioRespuestaDto
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}
