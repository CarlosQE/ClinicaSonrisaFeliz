using ClinicaDental.Contrato.DTOs.HorarioOdontologo;
using ClinicaDental.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    /// <summary>
    /// Parte B del módulo 2: endpoints de horarios de trabajo de los odontólogos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HorarioOdontologoController : ControllerBase
    {
        private readonly IHorarioOdontologo _horarioServicio;

        public HorarioOdontologoController(IHorarioOdontologo horarioServicio)
        {
            _horarioServicio = horarioServicio;
        }

        [HttpGet("odontologo/{idOdontologo:guid}", Name = "ListarHorariosOdontologo")]
        public IActionResult GetPorOdontologo(Guid idOdontologo)
        {
            if (idOdontologo == Guid.Empty) return BadRequest("El id del odontólogo no es válido.");
            if (!_horarioServicio.ExisteOdontologo(idOdontologo))
                return NotFound($"No se encontró el odontólogo con el id: {idOdontologo}");
            return Ok(_horarioServicio.ObtenerPorOdontologo(idOdontologo));
        }

        [HttpGet("{id:guid}", Name = "ObtenerHorarioPorId")]
        public IActionResult Get(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            var horario = _horarioServicio.ObtenerPorId(id);
            if (horario == null)
                return NotFound($"No se encontró el horario con el id: {id}");
            return Ok(horario);
        }

        [HttpPost("odontologo/{idOdontologo:guid}", Name = "RegistrarHorarioOdontologo")]
        public IActionResult Post(Guid idOdontologo, [FromBody] HorarioOdontologoDto horarioDto)
        {
            if (idOdontologo == Guid.Empty) return BadRequest("El id del odontólogo no es válido.");
            if (horarioDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (horarioDto.HoraInicio == null || horarioDto.HoraFin == null)
                return BadRequest("La hora de inicio y la hora de fin son obligatorias.");

            if (!_horarioServicio.ExisteOdontologo(idOdontologo))
                return NotFound($"No se encontró el odontólogo con el id: {idOdontologo}");
            if (!_horarioServicio.DiaSemanaValido(horarioDto.DiaSemana))
                return BadRequest("El día de la semana no es válido.");
            if (!_horarioServicio.RangoValido(horarioDto.HoraInicio.Value, horarioDto.HoraFin.Value))
                return BadRequest("El horario debe estar entre 06:00 y 22:00, con hora fin mayor a inicio y duración mínima de 1 hora.");
            if (_horarioServicio.SeSolapa(idOdontologo, horarioDto.DiaSemana, horarioDto.HoraInicio.Value, horarioDto.HoraFin.Value))
                return BadRequest("El horario se solapa con otra franja del mismo odontólogo.");

            try
            {
                var nuevo = _horarioServicio.Registrar(idOdontologo, horarioDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo registrar el horario por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el horario.");
            }
        }

        [HttpPut("{id:guid}", Name = "ActualizarHorarioOdontologo")]
        public IActionResult Put(Guid id, [FromBody] HorarioOdontologoDto horarioDto)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (horarioDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (horarioDto.HoraInicio == null || horarioDto.HoraFin == null)
                return BadRequest("La hora de inicio y la hora de fin son obligatorias.");

            var horario = _horarioServicio.ObtenerPorId(id);
            if (horario == null)
                return NotFound($"No se encontró el horario con el id: {id}");
            if (!_horarioServicio.DiaSemanaValido(horarioDto.DiaSemana))
                return BadRequest("El día de la semana no es válido.");
            if (!_horarioServicio.RangoValido(horarioDto.HoraInicio.Value, horarioDto.HoraFin.Value))
                return BadRequest("El horario debe estar entre 06:00 y 22:00, con hora fin mayor a inicio y duración mínima de 1 hora.");
            if (_horarioServicio.SeSolapa(horario.IdOdontologo, horarioDto.DiaSemana, horarioDto.HoraInicio.Value, horarioDto.HoraFin.Value, id))
                return BadRequest("El horario se solapa con otra franja del mismo odontólogo.");
            if (_horarioServicio.TieneCitasFuturas(id))
                return BadRequest("No se puede modificar un horario que tiene citas futuras.");

            try
            {
                var actualizado = _horarioServicio.Actualizar(id, horarioDto);
                return Ok(actualizado);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo actualizar el horario por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el horario.");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarHorarioOdontologo")]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (!_horarioServicio.Existe(id))
                return NotFound($"No se encontró el horario con el id: {id}");

            if (_horarioServicio.TieneCitasFuturas(id))
            {
                return BadRequest("No se puede eliminar un horario que tiene citas futuras.");
            }

            try
            {
                _horarioServicio.Eliminar(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al eliminar el horario.");
            }
        }
    }
}
