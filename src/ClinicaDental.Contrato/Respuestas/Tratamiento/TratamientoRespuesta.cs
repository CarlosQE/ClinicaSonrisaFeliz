using ClinicaDental.Contrato.Enums;
using ClinicaDental.Contrato.Respuestas.Cita;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Contrato.Respuestas.Paciente;

namespace ClinicaDental.Contrato.Respuestas.Tratamiento
{
    public class TratamientoRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        public Guid IdOdontologo { get; set; }
        public Guid? IdCita { get; set; }
        public PacienteRespuesta? Paciente { get; set; }
        public OdontologoRespuesta? Odontologo { get; set; }
        public CitaRespuesta? Cita { get; set; }
        public TipoTratamiento Tipo { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Costo { get; set; }
        public string? Observaciones { get; set; }
    }
}
