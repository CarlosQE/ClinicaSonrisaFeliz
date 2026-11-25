using ClinicaDental.Contrato.Enums;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Contrato.Respuestas.Paciente;

namespace ClinicaDental.Contrato.Respuestas.Cita
{
    public class CitaRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        public PacienteRespuesta? Paciente { get; set; }
        public Guid IdOdontologo { get; set; }
        public OdontologoRespuesta? Odontologo { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public int DuracionMinutos { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; }
        public string? MotivoCancelacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeOnly HoraFin { get; set; }
        public bool EstaVigente { get; set; }
    }
}
