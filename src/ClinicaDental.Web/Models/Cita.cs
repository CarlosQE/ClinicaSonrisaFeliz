using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaDental.Web.Models
{
    [Index(nameof(IdOdontologo), nameof(Fecha), nameof(Hora))]
    [Index(nameof(IdPaciente), nameof(Fecha), nameof(Hora))]
    public class Cita
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public virtual Paciente? Paciente { get; set; }
        public Guid IdOdontologo { get; set; }
        [ForeignKey(nameof(IdOdontologo))]
        public virtual Odontologo? Odontologo { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public int DuracionMinutos { get; set; } = 30;
        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [MaxLength(250)]
        public string Motivo { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; } = EstadoCita.Programada;
        [MaxLength(250)]
        public string? MotivoCancelacion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public TimeOnly HoraFin => Hora.AddMinutes(DuracionMinutos);
        public bool OcupaHorario => Estado != EstadoCita.Cancelada;
        public bool EstaVigente => Estado is EstadoCita.Programada or EstadoCita.Reprogramada;
    }
}
