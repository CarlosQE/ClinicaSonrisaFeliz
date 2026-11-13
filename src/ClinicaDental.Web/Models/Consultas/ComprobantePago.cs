namespace ClinicaDental.Web.Models.Consultas;

/// <summary>Datos para imprimir un comprobante (IPagoService.ObtenerComprobanteAsync, módulo 5).</summary>
public class ComprobantePago
{
    public int PagoId { get; init; }
    public string NumeroComprobante { get; init; } = string.Empty;
    public DateTime FechaPago { get; init; }
    public string PacienteNombre { get; init; } = string.Empty;
    public string PacienteCI { get; init; } = string.Empty;
    public TipoTratamiento Tipo { get; init; }
    public decimal CostoTratamiento { get; init; }
    public decimal Monto { get; init; }
    public MetodoPago Metodo { get; init; }

    /// <summary>Saldo que queda pendiente DESPUÉS de este pago.</summary>
    public decimal SaldoRestante { get; init; }
}
