using ClinicaDental.Web.DTOs.Paciente;
using ClinicaDental.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly IPaciente _pacienteServicio;

        public PacienteController(IPaciente pacienteServicio)
        {
            _pacienteServicio = pacienteServicio;
        }

        [HttpGet(Name = "ListarPacientes")]
        public IActionResult Get()
        {
            return Ok(_pacienteServicio.ObtenerTodos());
        }

        [HttpGet("{id:guid}", Name = "ObtenerPacientePorId")]
        public IActionResult Get(Guid id)
        {
            var paciente = _pacienteServicio.ObtenerPorId(id);

            if (paciente == null)
            {
                return NotFound($"No se encontró el paciente con el id: {id}");
            }

            return Ok(paciente);
        }

        [HttpGet("{id:guid}/historial", Name = "ConsultarHistorialPaciente")]
        public IActionResult GetHistorial(Guid id)
        {
            var historial = _pacienteServicio.ObtenerHistorial(id);

            if (historial == null)
            {
                return NotFound($"No se encontró el paciente con el id: {id}");
            }

            return Ok(historial);
        }

        [HttpPost(Name = "RegistrarPaciente")]
        public IActionResult Post([FromBody] PacienteDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pacienteServicio.DatosObligatoriosValidos(
                    pacienteDto.Nombres,
                    pacienteDto.Apellidos,
                    pacienteDto.CI,
                    pacienteDto.Telefono,
                    pacienteDto.FechaNacimiento))
            {
                return BadRequest("Nombres, apellidos, CI, fecha de nacimiento y teléfono son obligatorios.");
            }

            if (!_pacienteServicio.FechaNacimientoValida(pacienteDto.FechaNacimiento))
            {
                return BadRequest("La fecha de nacimiento no puede ser futura.");
            }

            if (_pacienteServicio.CiRepetido(pacienteDto.CI))
            {
                return BadRequest("El CI ya está registrado.");
            }

            try
            {
                var nuevo = _pacienteServicio.Registrar(pacienteDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el paciente.");
            }
        }

        [HttpPut("{id:guid}", Name = "ActualizarPaciente")]
        public IActionResult Put(Guid id, [FromBody] ActualizarPacienteDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pacienteServicio.ExistePaciente(id))
            {
                return NotFound($"No se encontró el paciente con el id: {id}");
            }

            if (!_pacienteServicio.DatosObligatoriosValidos(
                    pacienteDto.Nombres,
                    pacienteDto.Apellidos,
                    pacienteDto.CI,
                    pacienteDto.Telefono,
                    pacienteDto.FechaNacimiento))
            {
                return BadRequest("Nombres, apellidos, CI, fecha de nacimiento y teléfono son obligatorios.");
            }

            if (!_pacienteServicio.FechaNacimientoValida(pacienteDto.FechaNacimiento))
            {
                return BadRequest("La fecha de nacimiento no puede ser futura.");
            }

            if (_pacienteServicio.CiRepetido(pacienteDto.CI, id))
            {
                return BadRequest("El CI ya está registrado por otro paciente.");
            }

            try
            {
                var actualizado = _pacienteServicio.Actualizar(id, pacienteDto);
                return Ok(actualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el paciente.");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarPaciente")]
        public IActionResult Delete(Guid id)
        {
            if (!_pacienteServicio.ExistePaciente(id))
            {
                return NotFound($"No se encontró el paciente con el id: {id}");
            }

            if (_pacienteServicio.TieneCitas(id))
            {
                return BadRequest("No se puede eliminar el paciente porque tiene citas registradas.");
            }

            if (_pacienteServicio.TieneTratamientos(id))
            {
                return BadRequest("No se puede eliminar el paciente porque tiene tratamientos registrados.");
            }

            try
            {
                _pacienteServicio.Eliminar(id);
                return Ok("Paciente eliminado correctamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al eliminar el paciente.");
            }
        }
    }
}
