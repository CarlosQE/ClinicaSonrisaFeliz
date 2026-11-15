using CitaEntidad = ClinicaDental.Web.Models.Cita;
using PacienteEntidad = ClinicaDental.Web.Models.Paciente;
using TratamientoEntidad = ClinicaDental.Web.Models.Tratamiento;

namespace ClinicaDental.Web.DTOs.Paciente
{
    public class HistorialPacienteDto
    {
        public PacienteEntidad Paciente { get; set; } = null!;
        public IEnumerable<CitaEntidad> Citas { get; set; } = new List<CitaEntidad>();
        public IEnumerable<TratamientoEntidad> Tratamientos { get; set; } = new List<TratamientoEntidad>();
    }
}
