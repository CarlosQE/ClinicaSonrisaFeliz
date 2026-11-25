using ClinicaDental.Contrato.Enums;
using ClinicaDental.Contrato.Respuestas.Tratamiento;

namespace ClinicaDental.Contrato.Respuestas.Pago
{
    public class PagoRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdTratamiento { get; set; }
        public TratamientoRespuesta? Tratamiento { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago Metodo { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public string? Observacion { get; set; }
    }
}
