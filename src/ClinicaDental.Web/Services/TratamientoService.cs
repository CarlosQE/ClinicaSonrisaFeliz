using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;

namespace ClinicaDental.Web.Services;

/// <summary>
/// Módulo 4 - Gestión de Tratamientos. RESPONSABLE: Ultimo.
/// TODO(Ultimo): reemplazar cada NotImplementedException por la implementación real.
/// Guía: el módulo de Citas (CitaService.cs) es la implementación de referencia.
/// Ya tienes inyectado ICitaService para marcar la cita como Atendida al registrar un tratamiento.
/// Verifica tu avance con:  dotnet test --filter "Modulo=Tratamientos"
/// </summary>
public class TratamientoService : ITratamientoService
{
    private readonly ClinicaDbContext _db;
    private readonly ICitaService _citas;

    public TratamientoService(ClinicaDbContext db, ICitaService citas)
    {
        _db = db;
        _citas = citas;
    }

    public Task<List<Tratamiento>> ListarAsync(int? pacienteId = null, TipoTratamiento? tipo = null)
        => throw new NotImplementedException("Módulo Tratamientos pendiente (Ultimo).");

    public Task<Tratamiento?> ObtenerPorIdAsync(int id)
        => throw new NotImplementedException("Módulo Tratamientos pendiente (Ultimo).");

    public Task<Tratamiento> RegistrarAsync(Tratamiento tratamiento)
        => throw new NotImplementedException("Módulo Tratamientos pendiente (Ultimo).");

    public Task ActualizarObservacionesAsync(int tratamientoId, string? observaciones)
        => throw new NotImplementedException("Módulo Tratamientos pendiente (Ultimo).");

    public Task EliminarAsync(int id)
        => throw new NotImplementedException("Módulo Tratamientos pendiente (Ultimo).");
}
