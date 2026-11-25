using ClinicaDental.Contrato.DTOs.Pago;
using ClinicaDental.Contrato.Respuestas.Pago;

namespace ClinicaDental.Web.Interfaces
{
    public interface IPago
    {
        IEnumerable<PagoRespuesta> ObtenerTodos(Guid? idPaciente = null);
        PagoRespuesta? ObtenerPorId(Guid id);
        PagoRespuesta registrar(PagoDto pago);
        IEnumerable<DeudaRespuesta> ConsultarDeudas(Guid? idPaciente = null);
        decimal ObtenerSaldoPendiente(Guid idTratamiento);
        ComprobanteRespuesta? ObtenerComprobante(Guid idPago);
        bool ExistePaciente(Guid idPaciente);
        bool ExisteTratamiento(Guid idTratamiento);
        bool MontoValido(decimal monto);
        bool MetodoValido(MetodoPago metodo);
        bool MontoNoExcedeSaldo(Guid idTratamiento, decimal monto);
        bool ObservacionValida(string? observacion);
    }
}
