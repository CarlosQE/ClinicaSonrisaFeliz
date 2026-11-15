using ClinicaDental.Web.DTOs.Pago;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    public interface IPago
    {
        IEnumerable<Pago> ObtenerTodos(Guid? idPaciente = null);
        Pago? ObtenerPorId(Guid id);
        Pago registrar(PagoDto pago);
        IEnumerable<DeudaDto> ConsultarDeudas(Guid? idPaciente = null);
        decimal ObtenerSaldoPendiente(Guid idTratamiento);
        ComprobanteDto? ObtenerComprobante(Guid idPago);
        bool ExistePaciente(Guid idPaciente);
        bool ExisteTratamiento(Guid idTratamiento);
        bool MontoValido(decimal monto);
        bool MetodoValido(MetodoPago metodo);
        bool MontoNoExcedeSaldo(Guid idTratamiento, decimal monto);
    }
}