namespace ClinicaDental.Web.Models.Consultas;

/// <summary>Resultado de IPacienteService.ObtenerHistorialAsync (módulo 1).</summary>
public class HistorialPaciente
{
    public required Paciente Paciente { get; init; }

    /// <summary>Todas las citas del paciente (cualquier estado), de la más reciente a la más antigua.</summary>
    public List<Cita> Citas { get; init; } = new();

    /// <summary>Todos los tratamientos del paciente, del más reciente al más antiguo.</summary>
    public List<Tratamiento> Tratamientos { get; init; } = new();
}
