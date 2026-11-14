using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Cita
{
    public class CitaDto
    {
        [Required(ErrorMessage = "El paciente es requerido")]
        public Guid IdPaciente { get; set; }

        [Required(ErrorMessage = "El odontólogo es requerido")]
        public Guid IdOdontologo { get; set; }

        [Required(ErrorMessage = "La fecha y hora son requeridas")]
        public DateTime FechaHora { get; set; }

        public int DuracionMinutos { get; set; } = 30;

        [Required(ErrorMessage = "El motivo de la cita es requerido")]
        [MaxLength(250, ErrorMessage = "El motivo no puede tener más de 250 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }
}
