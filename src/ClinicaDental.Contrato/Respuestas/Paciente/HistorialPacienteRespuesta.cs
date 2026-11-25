using ClinicaDental.Contrato.Respuestas.Cita;
using ClinicaDental.Contrato.Respuestas.Tratamiento;

namespace ClinicaDental.Contrato.Respuestas.Paciente
{
    /// <summary>GET /api/Paciente/{id}/historial</summary>
    public class HistorialPacienteRespuesta
    {
        public PacienteRespuesta Paciente { get; set; } = new();
        public List<CitaRespuesta> Citas { get; set; } = new();
        public List<TratamientoRespuesta> Tratamientos { get; set; } = new();
    }
}
