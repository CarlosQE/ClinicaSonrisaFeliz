using ClinicaDental.Contrato.DTOs.Especialidad;
using ClinicaDental.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    /// <summary>
    /// Parte B del módulo 2: endpoints de especialidades.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EspecialidadController : ControllerBase
    {
        private readonly IEspecialidad _especialidadServicio;

        public EspecialidadController(IEspecialidad especialidadServicio)
        {
            _especialidadServicio = especialidadServicio;
        }

        [HttpGet(Name = "ListarEspecialidades")]
        public IActionResult Get()
        {
            return Ok(_especialidadServicio.ObtenerTodas());
        }

        [HttpGet("{id:guid}", Name = "ObtenerEspecialidadPorId")]
        public IActionResult Get(Guid id)
        {
            var especialidad = _especialidadServicio.ObtenerPorId(id);

            if (especialidad == null)
            {
                return NotFound($"No se encontró la especialidad con el id: {id}");
            }

            return Ok(especialidad);
        }

        [HttpPost(Name = "RegistrarEspecialidad")]
        public IActionResult Post([FromBody] EspecialidadDto especialidadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_especialidadServicio.NombreValido(especialidadDto.Nombre))
            {
                return BadRequest("El nombre de la especialidad es obligatorio.");
            }

            if (_especialidadServicio.NombreRepetido(especialidadDto.Nombre))
            {
                return BadRequest("La especialidad ya está registrada.");
            }

            try
            {
                var nueva = _especialidadServicio.Registrar(especialidadDto);
                return CreatedAtAction(nameof(Get), new { id = nueva.Id }, nueva);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar la especialidad.");
            }
        }

        [HttpPut("{id:guid}", Name = "ActualizarEspecialidad")]
        public IActionResult Put(Guid id, [FromBody] EspecialidadDto especialidadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_especialidadServicio.ObtenerPorId(id) == null)
            {
                return NotFound($"No se encontró la especialidad con el id: {id}");
            }

            if (!_especialidadServicio.NombreValido(especialidadDto.Nombre))
            {
                return BadRequest("El nombre de la especialidad es obligatorio.");
            }

            if (_especialidadServicio.NombreRepetido(especialidadDto.Nombre, id))
            {
                return BadRequest("La especialidad ya está registrada.");
            }

            try
            {
                var actualizada = _especialidadServicio.Actualizar(id, especialidadDto);
                return Ok(actualizada);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar la especialidad.");
            }
        }

        [HttpDelete("{id:guid}", Name = "EliminarEspecialidad")]
        public IActionResult Delete(Guid id)
        {
            if (_especialidadServicio.ObtenerPorId(id) == null)
            {
                return NotFound($"No se encontró la especialidad con el id: {id}");
            }

            if (_especialidadServicio.TieneOdontologos(id))
            {
                return BadRequest("No se puede eliminar una especialidad que tiene odontólogos asignados.");
            }

            try
            {
                _especialidadServicio.Eliminar(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al eliminar la especialidad.");
            }
        }
    }
}
