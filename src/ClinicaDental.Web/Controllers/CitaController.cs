using ClinicaDental.Web.DTOs.Cita;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly ICita _citaServicio;

        public CitaController(ICita citaServicio)
        {
            _citaServicio = citaServicio;
        }

        [HttpGet(Name = "ListarCitas")]
        public IActionResult Get()
        {
            var citas = _citaServicio.ObtenerTodos();
            return Ok(citas);
        }

        [HttpGet("{id:guid}", Name = "ObtenerCitaPorId")]
        public IActionResult Get(Guid id)
        {
            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
            {
                return NotFound($"No se encontró la cita con el id: {id}");
            }
            return Ok(cita);
        }

        [HttpGet("agenda", Name = "ConsultarAgendaDiaria")]
        public IActionResult GetAgenda([FromQuery] DateOnly? fecha, [FromQuery] Guid? idOdontologo, [FromQuery] bool incluirCanceladas = false)
        {
            if (!fecha.HasValue)
            {
                return BadRequest("La fecha es requerida (formato yyyy-MM-dd)");
            }

            if (idOdontologo.HasValue && !_citaServicio.ExisteOdontologo(idOdontologo.Value))
            {
                return BadRequest("El odontólogo indicado no existe");
            }

            var agenda = _citaServicio.ObtenerAgendaDiaria(fecha.Value, idOdontologo, incluirCanceladas);
            return Ok(agenda);
        }

        [HttpPost(Name = "AgendarCita")]
        public IActionResult Post([FromBody] CitaDto citaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_citaServicio.ExistePaciente(citaDto.IdPaciente))
            {
                return BadRequest("El paciente seleccionado no existe");
            }

            if (!_citaServicio.ExisteOdontologo(citaDto.IdOdontologo))
            {
                return BadRequest("El odontólogo seleccionado no existe");
            }

            if (!_citaServicio.OdontologoActivo(citaDto.IdOdontologo))
            {
                return BadRequest("El odontólogo está inactivo y no puede recibir citas");
            }

            if (!_citaServicio.DuracionValida(citaDto.DuracionMinutos))
            {
                return BadRequest("La duración debe estar entre 15 y 240 minutos");
            }

            var fecha = citaDto.Fecha!.Value;
            var hora = citaDto.Hora!.Value;

            if (!_citaServicio.FechaFutura(fecha, hora))
            {
                return BadRequest("La cita debe programarse para una fecha y hora futura");
            }

            if (!_citaServicio.DentroDeHorario(citaDto.IdOdontologo, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El horario elegido está fuera del horario de trabajo del odontólogo");
            }

            if (!_citaServicio.OdontologoDisponible(citaDto.IdOdontologo, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El odontólogo ya tiene una cita en ese horario");
            }

            if (!_citaServicio.PacienteDisponible(citaDto.IdPaciente, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El paciente ya tiene otra cita en ese horario");
            }

            try
            {
                var nuevaCita = _citaServicio.agendar(citaDto);
                return CreatedAtAction(nameof(Get), new { id = nuevaCita.Id }, nuevaCita);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al agendar la cita: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/reprogramar", Name = "ReprogramarCita")]
        public IActionResult Reprogramar(Guid id, [FromBody] ReprogramarCitaDto reprogramarDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
            {
                return NotFound($"No se encontró la cita con el id: {id}");
            }

            if (!cita.EstaVigente)
            {
                return BadRequest($"No se puede reprogramar una cita en estado {cita.Estado}");
            }

            var idOdontologo = reprogramarDto.IdNuevoOdontologo ?? cita.IdOdontologo;

            if (!_citaServicio.ExisteOdontologo(idOdontologo))
            {
                return BadRequest("El odontólogo seleccionado no existe");
            }

            if (!_citaServicio.OdontologoActivo(idOdontologo))
            {
                return BadRequest("El odontólogo está inactivo y no puede recibir citas");
            }

            var nuevaFecha = reprogramarDto.NuevaFecha!.Value;
            var nuevaHora = reprogramarDto.NuevaHora!.Value;

            if (!_citaServicio.FechaFutura(nuevaFecha, nuevaHora))
            {
                return BadRequest("La cita debe programarse para una fecha y hora futura");
            }

            if (!_citaServicio.DentroDeHorario(idOdontologo, nuevaFecha, nuevaHora, cita.DuracionMinutos))
            {
                return BadRequest("El horario elegido está fuera del horario de trabajo del odontólogo");
            }

            if (!_citaServicio.OdontologoDisponible(idOdontologo, nuevaFecha, nuevaHora, cita.DuracionMinutos, cita.Id))
            {
                return BadRequest("El odontólogo ya tiene una cita en ese horario");
            }

            if (!_citaServicio.PacienteDisponible(cita.IdPaciente, nuevaFecha, nuevaHora, cita.DuracionMinutos, cita.Id))
            {
                return BadRequest("El paciente ya tiene otra cita en ese horario");
            }

            try
            {
                var citaReprogramada = _citaServicio.reprogramar(id, nuevaFecha, nuevaHora, reprogramarDto.IdNuevoOdontologo);
                return Ok(citaReprogramada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al reprogramar la cita: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/cancelar", Name = "CancelarCita")]
        public IActionResult Cancelar(Guid id, [FromBody] CancelarCitaDto cancelarDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
            {
                return NotFound($"No se encontró la cita con el id: {id}");
            }

            if (!cita.EstaVigente)
            {
                return BadRequest($"No se puede cancelar una cita en estado {cita.Estado}");
            }

            try
            {
                _citaServicio.cancelar(id, cancelarDto.Motivo);
                return Ok($"La cita {id} fue cancelada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al cancelar la cita: {ex.Message}");
            }
        }
    }
}
