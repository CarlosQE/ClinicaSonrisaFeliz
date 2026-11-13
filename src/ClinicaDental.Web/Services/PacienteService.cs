using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Models.Consultas;
using ClinicaDental.Web.Services.Contratos;

namespace ClinicaDental.Web.Services;

/// <summary>
/// Módulo 1 - Gestión de Pacientes. RESPONSABLE: ApostolesEvoMorales.
/// TODO(ApostolesEvoMorales): reemplazar cada NotImplementedException por la implementación real.
/// Guía: el módulo de Citas (CitaService.cs) es la implementación de referencia.
/// Verifica tu avance con:  dotnet test --filter "Modulo=Pacientes"
/// </summary>
public class PacienteService : IPacienteService
{
    private readonly ClinicaDbContext _db;
    private readonly TimeProvider _reloj;

    public PacienteService(ClinicaDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public Task<List<Paciente>> ListarAsync(string? busqueda = null)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");

    public Task<Paciente?> ObtenerPorIdAsync(int id)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");

    public Task<Paciente> RegistrarAsync(Paciente paciente)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");

    public Task ActualizarAsync(Paciente paciente)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");

    public Task EliminarAsync(int id)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");

    public Task<HistorialPaciente> ObtenerHistorialAsync(int pacienteId)
        => throw new NotImplementedException("Módulo Pacientes pendiente (ApostolesEvoMorales).");
}
