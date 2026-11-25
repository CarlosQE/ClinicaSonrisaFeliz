using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Auth
{
    /// <summary>Cuerpo de PUT /api/Auth/usuarios/{id}/estado.</summary>
    public class CambiarEstadoUsuarioDto
    {
        [Required(ErrorMessage = "Debe indicar si el usuario queda activo o inactivo.")]
        public bool? Activo { get; set; }
    }
}
