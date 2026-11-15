namespace ClinicaDental.Web.DTOs.Pago
{
    public class ComprobanteDto
    {
        public Guid IdPago { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCI { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public DateOnly FechaTratamiento { get; set; }
        public decimal CostoTratamiento { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public decimal SaldoRestante { get; set; }
    }
}