namespace ClinicaDental.Web.Models.Consultas;

/// <summary>Una fila del reporte de deudas (IPagoService.ConsultarDeudasAsync, módulo 5).</summary>
public class DeudaPendiente
{
    public int TratamientoId { get; init; }
    public int PacienteId { get; init; }
    public string PacienteNombre { get; init; } = string.Empty;
    public TipoTratamiento Tipo { get; init; }
    public DateOnly Fecha { get; init; }
    public decimal Costo { get; init; }
    public decimal TotalPagado { get; init; }
    public decimal Saldo => Costo - TotalPagado;
}
