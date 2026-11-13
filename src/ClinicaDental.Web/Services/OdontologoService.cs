using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;

namespace ClinicaDental.Web.Services;

/// <summary>
/// Módulo 2 - Gestión de Odontólogos. RESPONSABLE: BagayerosChaco.
/// TODO(BagayerosChaco): reemplazar cada NotImplementedException por la implementación real.
/// Guía: el módulo de Citas (CitaService.cs) es la implementación de referencia.
/// Verifica tu avance con:  dotnet test --filter "Modulo=Odontologos"
/// </summary>
public class OdontologoService : IOdontologoService
{
    private readonly ClinicaDbContext _db;

    public OdontologoService(ClinicaDbContext db)
    {
        _db = db;
    }

    public Task<List<Odontologo>> ListarAsync(bool soloActivos = false)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<Odontologo?> ObtenerPorIdAsync(int id)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<Odontologo> RegistrarAsync(Odontologo odontologo)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task ActualizarAsync(Odontologo odontologo)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task CambiarEstadoAsync(int id, bool activo)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<List<Especialidad>> ListarEspecialidadesAsync()
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<Especialidad> RegistrarEspecialidadAsync(Especialidad especialidad)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<List<HorarioOdontologo>> ListarHorariosAsync(int odontologoId)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task<HorarioOdontologo> AgregarHorarioAsync(HorarioOdontologo horario)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");

    public Task EliminarHorarioAsync(int horarioId)
        => throw new NotImplementedException("Módulo Odontólogos pendiente (BagayerosChaco).");
}
