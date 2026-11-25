using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Comun;

namespace ClinicaDental.Contrato.DTOs.Auth
{
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
}
