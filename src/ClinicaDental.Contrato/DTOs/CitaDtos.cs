using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs
{
    /// <summary>Cuerpo de POST /api/Cita.</summary>
    public class CitaDto
    {
        [Required(ErrorMessage = "El paciente es requerido")]
        [Display(Name = "Paciente")]
        public Guid? IdPaciente { get; set; }

        [Required(ErrorMessage = "El odontólogo es requerido")]
        [Display(Name = "Odontólogo")]
        public Guid? IdOdontologo { get; set; }

        [Required(ErrorMessage = "La fecha y hora son requeridas")]
        [Display(Name = "Fecha y hora")]
        public DateTime? FechaHora { get; set; }

        [Range(15, 240, ErrorMessage = "La duración debe estar entre 15 y 240 minutos")]
        [Display(Name = "Duración (minutos)")]
        public int DuracionMinutos { get; set; } = 30;

        [Required(ErrorMessage = "El motivo de la cita es requerido")]
        [MaxLength(250, ErrorMessage = "El motivo no puede tener más de 250 caracteres")]
        [Display(Name = "Motivo de la consulta")]
        public string Motivo { get; set; } = string.Empty;
    }

    /// <summary>Cuerpo de PUT /api/Cita/{id}/reprogramar.</summary>
    public class ReprogramarCitaDto
    {
        [Required(ErrorMessage = "La nueva fecha y hora son requeridas")]
        [Display(Name = "Nueva fecha y hora")]
        public DateTime? NuevaFechaHora { get; set; }

        [Display(Name = "Odontólogo")]
        public Guid? IdNuevoOdontologo { get; set; }
    }

    /// <summary>Cuerpo de PUT /api/Cita/{id}/cancelar.</summary>
    public class CancelarCitaDto
    {
        [Required(ErrorMessage = "El motivo de la cancelación es requerido")]
        [MaxLength(250, ErrorMessage = "El motivo no puede tener más de 250 caracteres")]
        [Display(Name = "Motivo de la cancelación")]
        public string Motivo { get; set; } = string.Empty;
    }
}
