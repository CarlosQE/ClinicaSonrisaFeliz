using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Models.Consultas;
using ClinicaDental.Web.Services.Contratos;

namespace ClinicaDental.Web.Services;

/// <summary>
/// Módulo 5 - Gestión de Pagos. RESPONSABLE: GuaposMisteriosos.
/// TODO(GuaposMisteriosos): reemplazar cada NotImplementedException por la implementación real.
/// Guía: el módulo de Citas (CitaService.cs) es la implementación de referencia.
/// Verifica tu avance con:  dotnet test --filter "Modulo=Pagos"
/// </summary>
public class PagoService : IPagoService
{
    private readonly ClinicaDbContext _db;
    private readonly TimeProvider _reloj;

    public PagoService(ClinicaDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public Task<List<Pago>> ListarAsync(int? pacienteId = null)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");

    public Task<Pago?> ObtenerPorIdAsync(int id)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");

    public Task<Pago> RegistrarAsync(Pago pago)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");

    public Task<decimal> ObtenerSaldoPendienteAsync(int tratamientoId)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");

    public Task<List<DeudaPendiente>> ConsultarDeudasAsync(int? pacienteId = null)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");

    public Task<ComprobantePago> ObtenerComprobanteAsync(int pagoId)
        => throw new NotImplementedException("Módulo Pagos pendiente (GuaposMisteriosos).");
}
