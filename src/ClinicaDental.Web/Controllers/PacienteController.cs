using ClinicaDental.Contrato.DTOs.Paciente;
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
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
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
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
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
            if (pacienteDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_pacienteServicio.NombreValido(pacienteDto.Nombres) || !_pacienteServicio.NombreValido(pacienteDto.Apellidos))
                return BadRequest("Nombres y apellidos deben tener entre 2 y 50 caracteres, solo letras, espacios, apóstrofe o guion.");
            if (!_pacienteServicio.CiValido(pacienteDto.CI))
                return BadRequest("El CI debe tener 5 a 10 dígitos, con complemento opcional (ej. 1234567 o 1234567-1A).");
            if (pacienteDto.FechaNacimiento == null || pacienteDto.FechaNacimiento == default)
                return BadRequest("La fecha de nacimiento es obligatoria.");
            if (!_pacienteServicio.FechaNacimientoValida(pacienteDto.FechaNacimiento.Value))
                return BadRequest("La fecha de nacimiento no es válida (no futura, desde 1900-01-01, edad máxima 120 años).");
            if (!_pacienteServicio.TelefonoValido(pacienteDto.Telefono))
                return BadRequest("El teléfono debe tener 7 a 15 dígitos, con + opcional.");
            if (!_pacienteServicio.EmailValido(pacienteDto.Email))
                return BadRequest("El email no es válido o supera los 100 caracteres.");
            if (!_pacienteServicio.DireccionValida(pacienteDto.Direccion))
                return BadRequest("La dirección no puede tener más de 150 caracteres ni caracteres de control.");

            if (_pacienteServicio.CiRepetido(pacienteDto.CI))
                return BadRequest("El CI ya está registrado.");

            try
            {
                var nuevo = _pacienteServicio.Registrar(pacienteDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo registrar el paciente por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el paciente.");
            }
        }

        [HttpPut("{id:guid}", Name = "ActualizarPaciente")]
        public IActionResult Put(Guid id, [FromBody] PacienteDto pacienteDto)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
            if (pacienteDto == null) return BadRequest("El cuerpo de la solicitud es requerido.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_pacienteServicio.ExistePaciente(id))
                return NotFound($"No se encontró el paciente con el id: {id}");

            if (!_pacienteServicio.NombreValido(pacienteDto.Nombres) || !_pacienteServicio.NombreValido(pacienteDto.Apellidos))
                return BadRequest("Nombres y apellidos deben tener entre 2 y 50 caracteres, solo letras, espacios, apóstrofe o guion.");
            if (!_pacienteServicio.CiValido(pacienteDto.CI))
                return BadRequest("El CI debe tener 5 a 10 dígitos, con complemento opcional (ej. 1234567 o 1234567-1A).");
            if (pacienteDto.FechaNacimiento == null || pacienteDto.FechaNacimiento == default)
                return BadRequest("La fecha de nacimiento es obligatoria.");
            if (!_pacienteServicio.FechaNacimientoValida(pacienteDto.FechaNacimiento.Value))
                return BadRequest("La fecha de nacimiento no es válida (no futura, desde 1900-01-01, edad máxima 120 años).");
            if (!_pacienteServicio.TelefonoValido(pacienteDto.Telefono))
                return BadRequest("El teléfono debe tener 7 a 15 dígitos, con + opcional.");
            if (!_pacienteServicio.EmailValido(pacienteDto.Email))
                return BadRequest("El email no es válido o supera los 100 caracteres.");
            if (!_pacienteServicio.DireccionValida(pacienteDto.Direccion))
                return BadRequest("La dirección no puede tener más de 150 caracteres ni caracteres de control.");

            if (_pacienteServicio.CiRepetido(pacienteDto.CI, id))
                return BadRequest("El CI ya está registrado por otro paciente.");

            try
            {
                var actualizado = _pacienteServicio.Actualizar(id, pacienteDto);
                return Ok(actualizado);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict("No se pudo actualizar el paciente por un conflicto de datos.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el paciente.");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarPaciente")]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("El id no es válido.");
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
