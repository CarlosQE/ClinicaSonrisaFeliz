using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Cita
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

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha")]
        public DateOnly? Fecha { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        [Display(Name = "Hora")]
        public TimeOnly? Hora { get; set; }

        [Display(Name = "Duración (minutos)")]
        public int DuracionMinutos { get; set; } = 30;

        [Required(ErrorMessage = "El motivo de la cita es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres")]
        [RegularExpression(@"^(?=.*\p{L}).+$", ErrorMessage = "El motivo debe contener al menos una letra.")]
        [Display(Name = "Motivo de la consulta")]
        public string Motivo { get; set; } = string.Empty;
    }
}
