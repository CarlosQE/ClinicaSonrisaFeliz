using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Tratamiento
{
    /// <summary>Cuerpo de PUT /api/Tratamiento/{id}/observaciones.</summary>
    public class ActualizarObservacionesDto
    {
        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        [StringLength(500, ErrorMessage = "Las observaciones no pueden tener más de 500 caracteres.")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;
    }
}
