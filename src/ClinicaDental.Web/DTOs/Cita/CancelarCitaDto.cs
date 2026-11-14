using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Cita
{
    public class CancelarCitaDto
    {
        [Required(ErrorMessage = "El motivo de la cancelación es requerido")]
        [MaxLength(250, ErrorMessage = "El motivo no puede tener más de 250 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }
}
