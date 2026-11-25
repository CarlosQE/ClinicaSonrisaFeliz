using ClinicaDental.Contrato.DTOs.Pago;
using ClinicaDental.Web.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPago _pagoServicio;

        public PagoController(IPago pagoServicio)
        {
            _pagoServicio = pagoServicio;
        }

        [HttpGet(Name = "ListarPagos")]
        public IActionResult Get([FromQuery] Guid? idPaciente)
        {
            if (idPaciente.HasValue && !_pagoServicio.ExistePaciente(idPaciente.Value))
            {
                return BadRequest("El paciente indicado no existe");
            }

            var pagos = _pagoServicio.ObtenerTodos(idPaciente);
            return Ok(pagos);
        }

        [HttpGet("{id:guid}", Name = "ObtenerPagoPorId")]
        public IActionResult Get(Guid id)
        {
            var pago = _pagoServicio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound($"No se encontró el pago con el id: {id}");
            }
            return Ok(pago);
        }

        [HttpGet("deudas", Name = "ConsultarDeudasPendientes")]
        public IActionResult GetDeudas([FromQuery] Guid? idPaciente)
        {
            if (idPaciente.HasValue && !_pagoServicio.ExistePaciente(idPaciente.Value))
            {
                return BadRequest("El paciente indicado no existe");
            }

            var deudas = _pagoServicio.ConsultarDeudas(idPaciente);
            return Ok(deudas);
        }

        [HttpGet("tratamiento/{idTratamiento:guid}/saldo", Name = "ConsultarSaldoTratamiento")]
        public IActionResult GetSaldo(Guid idTratamiento)
        {
            if (!_pagoServicio.ExisteTratamiento(idTratamiento))
            {
                return NotFound($"No se encontró el tratamiento con el id: {idTratamiento}");
            }

            var saldo = _pagoServicio.ObtenerSaldoPendiente(idTratamiento);
            return Ok(new { idTratamiento, saldoPendiente = saldo });
        }

        [HttpGet("{id:guid}/comprobante", Name = "EmitirComprobante")]
        public IActionResult GetComprobante(Guid id)
        {
            var comprobante = _pagoServicio.ObtenerComprobante(id);
            if (comprobante == null)
            {
                return NotFound($"No se encontró el pago con el id: {id}");
            }
            return Ok(comprobante);
        }

        [HttpPost(Name = "RegistrarPago")]
        public IActionResult Post([FromBody] PagoDto pagoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pagoServicio.MontoValido(pagoDto.Monto))
            {
                return BadRequest("El monto debe ser mayor a 0, no mayor a 1.000.000 y tener como máximo 2 decimales");
            }

            if (!_pagoServicio.MetodoValido(pagoDto.Metodo))
            {
                return BadRequest("El método de pago no es válido (0 = Efectivo, 1 = Tarjeta, 2 = Transferencia, 3 = QR)");
            }

            if (!_pagoServicio.ExisteTratamiento(pagoDto.IdTratamiento!.Value))
            {
                return BadRequest("El tratamiento seleccionado no existe");
            }

            var saldo = _pagoServicio.ObtenerSaldoPendiente(pagoDto.IdTratamiento!.Value);
            if (saldo <= 0)
            {
                return BadRequest("El tratamiento ya fue pagado por completo");
            }

            if (!_pagoServicio.MontoNoExcedeSaldo(pagoDto.IdTratamiento!.Value, pagoDto.Monto))
            {
                return BadRequest($"El monto excede el saldo pendiente del tratamiento ({saldo} Bs)");
            }

            try
            {
                var nuevoPago = _pagoServicio.registrar(pagoDto);
                return CreatedAtAction(nameof(Get), new { id = nuevoPago.Id }, nuevoPago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar el pago: {ex.Message}");
            }
        }
    }
}