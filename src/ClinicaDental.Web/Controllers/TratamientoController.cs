using ClinicaDental.Web.DTOs.Tratamiento;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TratamientoController : ControllerBase
    {
        private readonly ITratamiento _tratamientoServicio;

        public TratamientoController(ITratamiento tratamientoServicio)
        {
            _tratamientoServicio = tratamientoServicio;
        }

        [HttpGet(Name = "ListarTratamientos")]
        public IActionResult Get([FromQuery] Guid? idPaciente, [FromQuery] TipoTratamiento? tipo, [FromQuery] Guid? idOdontologo)
        {
            if (idPaciente.HasValue && !_tratamientoServicio.ExistePaciente(idPaciente.Value))
            {
                return BadRequest("El paciente indicado no existe.");
            }

            if (idOdontologo.HasValue && !_tratamientoServicio.ExisteOdontologo(idOdontologo.Value))
            {
                return BadRequest("El odontólogo indicado no existe.");
            }

            if (tipo.HasValue && !_tratamientoServicio.TipoValido(tipo.Value))
            {
                return BadRequest("El tipo de tratamiento no es válido (0 = LimpiezaDental, 1 = Ortodoncia, 2 = Endodoncia, 3 = Extraccion).");
            }

            var tratamientos = _tratamientoServicio.ObtenerTodos(idPaciente, tipo, idOdontologo);
            return Ok(tratamientos);
        }

        [HttpGet("{id:guid}", Name = "ObtenerTratamientoPorId")]
        public IActionResult Get(Guid id)
        {
            var tratamiento = _tratamientoServicio.ObtenerPorId(id);

            if (tratamiento == null)
            {
                return NotFound($"No se encontró el tratamiento con el id: {id}");
            }

            return Ok(tratamiento);
        }

        [HttpPost(Name = "RegistrarTratamiento")]
        public IActionResult Post([FromBody] TratamientoDto tratamientoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_tratamientoServicio.ExistePaciente(tratamientoDto.IdPaciente))
            {
                return BadRequest("El paciente seleccionado no existe.");
            }

            if (!_tratamientoServicio.ExisteOdontologo(tratamientoDto.IdOdontologo))
            {
                return BadRequest("El odontólogo seleccionado no existe.");
            }

            if (!_tratamientoServicio.OdontologoActivo(tratamientoDto.IdOdontologo))
            {
                return BadRequest("El odontólogo está inactivo y no puede realizar tratamientos.");
            }

            if (!_tratamientoServicio.TipoValido(tratamientoDto.Tipo))
            {
                return BadRequest("El tipo de tratamiento no es válido (0 = LimpiezaDental, 1 = Ortodoncia, 2 = Endodoncia, 3 = Extraccion).");
            }

            if (!_tratamientoServicio.CostoValido(tratamientoDto.Costo))
            {
                return BadRequest("El costo debe ser mayor a 0 y tener como máximo 2 decimales.");
            }

            if (tratamientoDto.Fecha.HasValue && !_tratamientoServicio.FechaValida(tratamientoDto.Fecha.Value))
            {
                return BadRequest("La fecha del tratamiento no es válida.");
            }

            var idCita = tratamientoDto.IdCita == Guid.Empty ? null : tratamientoDto.IdCita;
            if (idCita.HasValue)
            {
                if (!_tratamientoServicio.ExisteCita(idCita.Value))
                {
                    return BadRequest("La cita indicada no existe.");
                }

                if (!_tratamientoServicio.CitaPerteneceAPaciente(idCita.Value, tratamientoDto.IdPaciente))
                {
                    return BadRequest("La cita indicada no pertenece al paciente seleccionado.");
                }

                if (!_tratamientoServicio.CitaNoCancelada(idCita.Value))
                {
                    return BadRequest("No se puede registrar un tratamiento para una cita cancelada.");
                }
            }

            try
            {
                var nuevo = _tratamientoServicio.Registrar(tratamientoDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar el tratamiento: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/observaciones", Name = "ActualizarObservacionesTratamiento")]
        public IActionResult ActualizarObservaciones(Guid id, [FromBody] ActualizarObservacionesDto observacionesDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_tratamientoServicio.ExisteTratamiento(id))
            {
                return NotFound($"No se encontró el tratamiento con el id: {id}");
            }

            if (!_tratamientoServicio.ObservacionesValidas(observacionesDto.Observaciones))
            {
                return BadRequest("Las observaciones son obligatorias y no pueden tener más de 1000 caracteres.");
            }

            try
            {
                var actualizado = _tratamientoServicio.ActualizarObservaciones(id, observacionesDto.Observaciones);
                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar las observaciones del tratamiento: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarTratamiento")]
        public IActionResult Delete(Guid id)
        {
            if (!_tratamientoServicio.ExisteTratamiento(id))
            {
                return NotFound($"No se encontró el tratamiento con el id: {id}");
            }

            if (_tratamientoServicio.TienePagos(id))
            {
                return BadRequest("No se puede eliminar el tratamiento porque tiene pagos registrados.");
            }

            try
            {
                _tratamientoServicio.Eliminar(id);
                return Ok("Tratamiento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar el tratamiento: {ex.Message}");
            }
        }
    }
}
