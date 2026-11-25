using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Odontologo
{
    /// <summary>Cuerpo de PUT /api/Odontologo/{id}/estado.</summary>
    public class CambiarEstadoOdontologoDto
    {
        [Required(ErrorMessage = "Debe indicar si el odontólogo queda activo o inactivo.")]
        public bool? Activo { get; set; }
    }
}
