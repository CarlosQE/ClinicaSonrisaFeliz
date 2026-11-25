using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Cita
{
    /// <summary>Cuerpo de DELETE /api/Cita/cancelar/{id}.</summary>
    public class CancelarCitaDto
    {
        [Required(ErrorMessage = "El motivo de la cancelación es requerido")]
        [MaxLength(250, ErrorMessage = "El motivo no puede tener más de 250 caracteres")]
        [Display(Name = "Motivo de la cancelación")]
        public string Motivo { get; set; } = string.Empty;
    }
}
