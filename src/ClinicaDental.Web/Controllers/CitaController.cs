using ClinicaDental.Contrato.DTOs.Cita;
using ClinicaDental.Web.Interfaces;
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
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
                return NotFound($"No se encontró la cita con el id: {id}");
            return Ok(cita);
        }

        [HttpGet("agenda", Name = "ConsultarAgendaDiaria")]
        public IActionResult GetAgenda([FromQuery] DateOnly? fecha, [FromQuery] Guid? idOdontologo, [FromQuery] bool incluirCanceladas = false)
        {
            if (!fecha.HasValue)
            {
                return BadRequest("La fecha es requerida (formato yyyy-MM-dd)");
            }

            if (idOdontologo.HasValue && idOdontologo.Value == Guid.Empty) return BadRequest("El id del odontólogo no es válido.");
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
            if (citaDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (citaDto.IdPaciente == null || citaDto.IdPaciente == Guid.Empty)
                return BadRequest("El id del paciente no es válido.");
            if (citaDto.IdOdontologo == null || citaDto.IdOdontologo == Guid.Empty)
                return BadRequest("El id del odontólogo no es válido.");
            if (!_citaServicio.ExistePaciente(citaDto.IdPaciente.Value))
                return BadRequest("El paciente seleccionado no existe");
            if (!_citaServicio.ExisteOdontologo(citaDto.IdOdontologo.Value))
                return BadRequest("El odontólogo seleccionado no existe");
            if (!_citaServicio.OdontologoActivo(citaDto.IdOdontologo.Value))
                return BadRequest("El odontólogo está inactivo y no puede recibir citas");
            if (!_citaServicio.DuracionValida(citaDto.DuracionMinutos))
                return BadRequest("La duración debe estar entre 15 y 240 minutos y ser múltiplo de 15.");
            if (!_citaServicio.MotivoValido(citaDto.Motivo))
                return BadRequest("El motivo debe tener entre 3 y 200 caracteres y contener al menos una letra.");

            var fecha = citaDto.Fecha!.Value;
            var hora = citaDto.Hora!.Value;

            if (!_citaServicio.FechaDentroDeRango(fecha, hora))
                return BadRequest("La cita debe programarse para una fecha y hora futura, con máximo 365 días de anticipación.");

            if (!_citaServicio.DentroDeHorario(citaDto.IdOdontologo!.Value, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El horario elegido está fuera del horario de trabajo del odontólogo (06:00 a 22:00).");
            }

            if (!_citaServicio.OdontologoDisponible(citaDto.IdOdontologo!.Value, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El odontólogo ya tiene una cita en ese horario");
            }

            if (!_citaServicio.PacienteDisponible(citaDto.IdPaciente!.Value, fecha, hora, citaDto.DuracionMinutos))
            {
                return BadRequest("El paciente ya tiene otra cita en ese horario");
            }

            try
            {
                var nuevaCita = _citaServicio.agendar(citaDto);
                return CreatedAtAction(nameof(Get), new { id = nuevaCita.Id }, nuevaCita);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo agendar la cita por un conflicto de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al agendar la cita: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/reprogramar", Name = "ReprogramarCita")]
        public IActionResult Reprogramar(Guid id, [FromBody] ReprogramarCitaDto reprogramarDto)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (reprogramarDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
                return NotFound($"No se encontró la cita con el id: {id}");
            if (!cita.EstaVigente)
                return BadRequest($"No se puede reprogramar una cita en estado {cita.Estado}");

            if (reprogramarDto.IdNuevoOdontologo.HasValue && reprogramarDto.IdNuevoOdontologo.Value == Guid.Empty)
                return BadRequest("El id del nuevo odontólogo no es válido.");

            var idOdontologo = reprogramarDto.IdNuevoOdontologo ?? cita.IdOdontologo;
            if (!_citaServicio.ExisteOdontologo(idOdontologo))
                return BadRequest("El odontólogo seleccionado no existe");
            if (!_citaServicio.OdontologoActivo(idOdontologo))
                return BadRequest("El odontólogo está inactivo y no puede recibir citas");

            var nuevaFecha = reprogramarDto.NuevaFecha!.Value;
            var nuevaHora = reprogramarDto.NuevaHora!.Value;

            if (!_citaServicio.FechaDentroDeRango(nuevaFecha, nuevaHora))
                return BadRequest("La cita debe programarse para una fecha y hora futura, con máximo 365 días de anticipación.");

            if (!_citaServicio.DentroDeHorario(idOdontologo, nuevaFecha, nuevaHora, cita.DuracionMinutos))
            {
                return BadRequest("El horario elegido está fuera del horario de trabajo del odontólogo (06:00 a 22:00).");
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
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo reprogramar la cita por un conflicto de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al reprogramar la cita: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}/cancelar", Name = "CancelarCita")]
        public IActionResult Cancelar(Guid id, [FromBody] CancelarCitaDto cancelarDto)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (cancelarDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_citaServicio.MotivoValido(cancelarDto.Motivo))
                return BadRequest("El motivo debe tener entre 3 y 200 caracteres y contener al menos una letra.");

            var cita = _citaServicio.ObtenerPorId(id);
            if (cita == null)
                return NotFound($"No se encontró la cita con el id: {id}");
            if (!cita.EstaVigente)
                return BadRequest($"No se puede cancelar una cita en estado {cita.Estado}");

            try
            {
                _citaServicio.cancelar(id, cancelarDto.Motivo);
                return Ok($"La cita {id} fue cancelada");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo cancelar la cita por un conflicto de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al cancelar la cita: {ex.Message}");
            }
        }
    }
}
