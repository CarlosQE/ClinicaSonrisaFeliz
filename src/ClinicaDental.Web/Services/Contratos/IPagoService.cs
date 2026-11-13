using ClinicaDental.Web.Models;
using ClinicaDental.Web.Models.Consultas;

namespace ClinicaDental.Web.Services.Contratos;

/// <summary>
/// CONTRATO del módulo 5 - Gestión de Pagos (grupo GuaposMisteriosos).
/// Firmas CONGELADAS: no cambiar nombres, parámetros ni tipos de retorno sin acuerdo del líder.
/// Pruebas que lo validan: tests/ClinicaDental.Tests/Modulos/Pagos
/// </summary>
public interface IPagoService
{
    /// <summary>
    /// Lista pagos (con Tratamiento y Tratamiento.Paciente) del más reciente al más antiguo.
    /// Si se indica <paramref name="pacienteId"/>, solo los de ese paciente.
    /// </summary>
    Task<List<Pago>> ListarAsync(int? pacienteId = null);

    /// <summary>Devuelve el pago con Tratamiento y Tratamiento.Paciente, o null si no existe.</summary>
    Task<Pago?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Registra un pago. Asigna NumeroComprobante correlativo con formato "CMP-000001", "CMP-000002", ...
    /// y FechaPago = ahora si viene en default.
    /// Reglas -> ReglaNegocioException: el tratamiento debe existir; Monto &gt; 0;
    /// Monto no puede superar el saldo pendiente del tratamiento.
    /// </summary>
    Task<Pago> RegistrarAsync(Pago pago);

    /// <summary>Saldo pendiente = Costo del tratamiento - suma de sus pagos. No existe -> NoEncontradoException.</summary>
    Task<decimal> ObtenerSaldoPendienteAsync(int tratamientoId);

    /// <summary>
    /// Deudas pendientes: una fila por cada tratamiento con saldo &gt; 0, ordenadas por PacienteNombre.
    /// Si se indica <paramref name="pacienteId"/>, solo las de ese paciente.
    /// </summary>
    Task<List<DeudaPendiente>> ConsultarDeudasAsync(int? pacienteId = null);

    /// <summary>
    /// Datos del comprobante de un pago (SaldoRestante = saldo inmediatamente después de ese pago).
    /// No existe -> NoEncontradoException.
    /// </summary>
    Task<ComprobantePago> ObtenerComprobanteAsync(int pagoId);
}
