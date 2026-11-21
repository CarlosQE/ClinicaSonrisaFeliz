using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs
{
    /// <summary>Cuerpo de POST /api/Auth/login.</summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede tener más de 100 caracteres.")]
        [Display(Name = "Contraseña")]
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

    /// <summary>Cuerpo de PUT /api/Auth/contrasena.</summary>
    public class CambiarContrasenaDto
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [Display(Name = "Contraseña actual")]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [ContrasenaSegura]
        [Display(Name = "Nueva contraseña")]
        public string ContrasenaNueva { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma la nueva contraseña.")]
        [Compare(nameof(ContrasenaNueva), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }

    /// <summary>Cuerpo de PUT /api/Auth/usuarios/{id}/estado.</summary>
    public class CambiarEstadoUsuarioDto
    {
        public bool? Activo { get; set; }
    }

    /// <summary>Roles válidos (los mismos que ClinicaDental.Web.Models.Roles).</summary>
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
        public const string Odontologo = "Odontologo";

        public static readonly string[] Todos = [Administrador, Recepcionista, Odontologo];

        public static string Texto(string rol) => rol == Odontologo ? "Odontólogo" : rol;
    }

    /// <summary>
    /// Misma política que la API: mínimo 8 caracteres con mayúscula, minúscula, número y símbolo.
    /// Se valida en el formulario para avisar antes de enviar; la API vuelve a validar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ContrasenaSeguraAttribute : ValidationAttribute
    {
        public ContrasenaSeguraAttribute()
            : base("La contraseña debe tener al menos 8 caracteres, con mayúscula, minúscula, número y símbolo.") { }

        public override bool IsValid(object? value)
        {
            if (value is not string c || c.Length == 0) return true; // de vacío se encarga [Required]
            return c.Length is >= 8 and <= 100
                && c.Any(char.IsUpper)
                && c.Any(char.IsLower)
                && c.Any(char.IsDigit)
                && c.Any(x => !char.IsLetterOrDigit(x));
        }
    }
}
