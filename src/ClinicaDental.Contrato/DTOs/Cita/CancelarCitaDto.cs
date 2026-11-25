using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Cita
{
    /// <summary>Cuerpo de DELETE /api/Cita/cancelar/{id}.</summary>
    public class CancelarCitaDto
    {
        [Required(ErrorMessage = "El motivo de la cancelación es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres")]
        [RegularExpression(@"^(?=.*\p{L}).+$", ErrorMessage = "El motivo debe contener al menos una letra.")]
        [Display(Name = "Motivo de la cancelación")]
        public string Motivo { get; set; } = string.Empty;
    }
}
