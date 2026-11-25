using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.HorarioOdontologo
{
    /// <summary>Cuerpo de POST /api/HorarioOdontologo/odontologo/{id} y PUT /api/HorarioOdontologo/{id}.</summary>
    public class HorarioOdontologoDto
    {
        [Display(Name = "Día")]
        public DayOfWeek DiaSemana { get; set; } = DayOfWeek.Monday;

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [Display(Name = "Hora de inicio")]
        public TimeOnly? HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [Display(Name = "Hora de fin")]
        public TimeOnly? HoraFin { get; set; }
    }
}
