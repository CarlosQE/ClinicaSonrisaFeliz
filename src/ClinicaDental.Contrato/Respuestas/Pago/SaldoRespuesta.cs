namespace ClinicaDental.Contrato.Respuestas.Pago
{
    /// <summary>GET /api/Pago/tratamiento/{id}/saldo</summary>
    public class SaldoRespuesta
    {
        public Guid IdTratamiento { get; set; }
        public decimal SaldoPendiente { get; set; }
    }
}
