using ClinicaDental.Contrato.DTOs.Tratamiento;
using ClinicaDental.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TratamientoController : ControllerBase
    {
        private readonly ITratamiento _tratamientoServicio;
        private readonly TimeProvider _reloj;

        public TratamientoController(ITratamiento tratamientoServicio, TimeProvider reloj)
        {
            _tratamientoServicio = tratamientoServicio;
            _reloj = reloj;
        }

        [HttpGet(Name = "ListarTratamientos")]
        public IActionResult Get([FromQuery] Guid? idPaciente, [FromQuery] TipoTratamiento? tipo, [FromQuery] Guid? idOdontologo)
        {
            if (idPaciente.HasValue && idPaciente.Value == Guid.Empty) return BadRequest("El id del paciente no es válido.");
            if (idPaciente.HasValue && !_tratamientoServicio.ExistePaciente(idPaciente.Value))
            {
                return BadRequest("El paciente indicado no existe.");
            }

            if (idOdontologo.HasValue && idOdontologo.Value == Guid.Empty) return BadRequest("El id del odontólogo no es válido.");
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
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            var tratamiento = _tratamientoServicio.ObtenerPorId(id);
            if (tratamiento == null)
                return NotFound($"No se encontró el tratamiento con el id: {id}");
            return Ok(tratamiento);
        }

        [HttpPost(Name = "RegistrarTratamiento")]
        public IActionResult Post([FromBody] TratamientoDto tratamientoDto)
        {
            if (tratamientoDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (tratamientoDto.IdPaciente == null || tratamientoDto.IdPaciente == Guid.Empty)
                return BadRequest("El id del paciente no es válido.");
            if (tratamientoDto.IdOdontologo == null || tratamientoDto.IdOdontologo == Guid.Empty)
                return BadRequest("El id del odontólogo no es válido.");
            if (!_tratamientoServicio.ExistePaciente(tratamientoDto.IdPaciente.Value))
                return BadRequest("El paciente seleccionado no existe.");
            if (!_tratamientoServicio.ExisteOdontologo(tratamientoDto.IdOdontologo.Value))
                return BadRequest("El odontólogo seleccionado no existe.");
            if (!_tratamientoServicio.OdontologoActivo(tratamientoDto.IdOdontologo.Value))
                return BadRequest("El odontólogo está inactivo y no puede realizar tratamientos.");
            if (!_tratamientoServicio.TipoValido(tratamientoDto.Tipo))
                return BadRequest("El tipo de tratamiento no es válido (0 = LimpiezaDental, 1 = Ortodoncia, 2 = Endodoncia, 3 = Extraccion).");
            if (!_tratamientoServicio.CostoValido(tratamientoDto.Costo))
                return BadRequest("El costo debe estar entre 0.01 y 1,000,000 Bs, con como máximo 2 decimales.");
            if (tratamientoDto.Fecha.HasValue && !_tratamientoServicio.FechaValida(tratamientoDto.Fecha.Value, tratamientoDto.IdPaciente.Value))
                return BadRequest("La fecha del tratamiento no es válida (no futura, desde 2000-01-01 y no anterior al nacimiento del paciente).");
            if (!_tratamientoServicio.ObservacionesOpcionalesValidas(tratamientoDto.Observaciones))
                return BadRequest("Si ingresa observaciones, deben tener entre 3 y 1000 caracteres y no contener '<', '>' ni caracteres de control.");

            if (tratamientoDto.IdCita.HasValue && tratamientoDto.IdCita.Value == Guid.Empty)
                return BadRequest("El id de la cita no es válido.");
            var idCita = tratamientoDto.IdCita;
            if (idCita.HasValue)
            {
                if (!_tratamientoServicio.ExisteCita(idCita.Value))
                    return BadRequest("La cita indicada no existe.");
                if (!_tratamientoServicio.CitaPerteneceAPaciente(idCita.Value, tratamientoDto.IdPaciente.Value))
                    return BadRequest("La cita indicada no pertenece al paciente seleccionado.");
                if (!_tratamientoServicio.CitaNoCancelada(idCita.Value))
                    return BadRequest("No se puede registrar un tratamiento para una cita cancelada.");
                if (!_tratamientoServicio.CitaNoFutura(idCita.Value))
                    return BadRequest("No se puede registrar un tratamiento para una cita futura.");
                if (!_tratamientoServicio.CitaCorrespondeAOdontologo(idCita.Value, tratamientoDto.IdOdontologo.Value))
                    return BadRequest("El odontólogo seleccionado no coincide con el odontólogo de la cita asociada.");
                var fechaEfectiva = tratamientoDto.Fecha ?? DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);
                if (!_tratamientoServicio.FechaCoherenteConCita(fechaEfectiva, idCita.Value))
                    return BadRequest("La fecha del tratamiento no puede ser anterior a la fecha de la cita asociada.");
            }

            if (_tratamientoServicio.ExisteDuplicado(tratamientoDto))
                return Conflict("Ya existe un tratamiento idéntico registrado.");

            try
            {
                var nuevo = _tratamientoServicio.Registrar(tratamientoDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo registrar el tratamiento por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el tratamiento.");
            }
        }

        [HttpPut("{id:guid}/observaciones", Name = "ActualizarObservacionesTratamiento")]
        public IActionResult ActualizarObservaciones(Guid id, [FromBody] ActualizarObservacionesDto observacionesDto)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (observacionesDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_tratamientoServicio.ExisteTratamiento(id))
                return NotFound($"No se encontró el tratamiento con el id: {id}");
            if (!_tratamientoServicio.ObservacionesValidas(observacionesDto.Observaciones))
                return BadRequest("Las observaciones son obligatorias, deben tener entre 3 y 1000 caracteres y no contener '<', '>' ni caracteres de control.");

            try
            {
                var actualizado = _tratamientoServicio.ActualizarObservaciones(id, observacionesDto.Observaciones);
                return Ok(actualizado);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo actualizar el tratamiento por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar las observaciones del tratamiento.");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarTratamiento")]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (!_tratamientoServicio.ExisteTratamiento(id))
                return NotFound($"No se encontró el tratamiento con el id: {id}");

            if (_tratamientoServicio.TienePagos(id))
            {
                return BadRequest("No se puede eliminar el tratamiento porque tiene pagos registrados.");
            }

            try
            {
                _tratamientoServicio.Eliminar(id);
                return Ok("Tratamiento eliminado correctamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al eliminar el tratamiento.");
            }
        }
    }
}
