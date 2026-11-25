using ClinicaDental.Contrato.DTOs.Odontologo;
using ClinicaDental.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers
{
    /// <summary>
    /// Parte A del módulo 2: endpoints de gestión de odontólogos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OdontologoController : ControllerBase
    {
        private readonly IOdontologo _odontologoServicio;

        public OdontologoController(IOdontologo odontologoServicio)
        {
            _odontologoServicio = odontologoServicio;
        }

        [HttpGet(Name = "ListarOdontologos")]
        public IActionResult Get([FromQuery] bool? activo, [FromQuery] Guid? idEspecialidad)
        {
            if (idEspecialidad.HasValue && !_odontologoServicio.ExisteEspecialidad(idEspecialidad.Value))
            {
                return BadRequest("La especialidad indicada no existe.");
            }

            return Ok(_odontologoServicio.ObtenerTodos(activo, idEspecialidad));
        }

        [HttpGet("{id:guid}", Name = "ObtenerOdontologoPorId")]
        public IActionResult Get(Guid id)
        {
            var odontologo = _odontologoServicio.ObtenerPorId(id);

            if (odontologo == null)
            {
                return NotFound($"No se encontró el odontólogo con el id: {id}");
            }

            return Ok(odontologo);
        }

        [HttpPost(Name = "RegistrarOdontologo")]
        public IActionResult Post([FromBody] OdontologoDto odontologoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_odontologoServicio.DatosObligatoriosValidos(
                    odontologoDto.Nombres,
                    odontologoDto.Apellidos,
                    odontologoDto.Matricula,
                    odontologoDto.Telefono))
            {
                return BadRequest("Nombres, apellidos, matrícula y teléfono son obligatorios.");
            }

            if (!_odontologoServicio.ExisteEspecialidad(odontologoDto.IdEspecialidad!.Value))
            {
                return BadRequest("La especialidad seleccionada no existe.");
            }

            if (_odontologoServicio.MatriculaRepetida(odontologoDto.Matricula))
            {
                return BadRequest("La matrícula ya está registrada.");
            }

            try
            {
                var nuevo = _odontologoServicio.Registrar(odontologoDto);
                return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el odontólogo.");
            }
        }

        [HttpPut("{id:guid}", Name = "ActualizarOdontologo")]
        public IActionResult Put(Guid id, [FromBody] OdontologoDto odontologoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_odontologoServicio.ExisteOdontologo(id))
            {
                return NotFound($"No se encontró el odontólogo con el id: {id}");
            }

            if (!_odontologoServicio.DatosObligatoriosValidos(
                    odontologoDto.Nombres,
                    odontologoDto.Apellidos,
                    odontologoDto.Matricula,
                    odontologoDto.Telefono))
            {
                return BadRequest("Nombres, apellidos, matrícula y teléfono son obligatorios.");
            }

            if (!_odontologoServicio.ExisteEspecialidad(odontologoDto.IdEspecialidad!.Value))
            {
                return BadRequest("La especialidad seleccionada no existe.");
            }

            if (_odontologoServicio.MatriculaRepetida(odontologoDto.Matricula, id))
            {
                return BadRequest("La matrícula ya está registrada por otro odontólogo.");
            }

            try
            {
                var actualizado = _odontologoServicio.Actualizar(id, odontologoDto);
                return Ok(actualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el odontólogo.");
            }
        }

        [HttpPut("{id:guid}/estado", Name = "CambiarEstadoOdontologo")]
        public IActionResult CambiarEstado(Guid id, [FromBody] CambiarEstadoOdontologoDto estadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!estadoDto.Activo.HasValue)
            {
                return BadRequest("Debe indicar si el odontólogo queda activo o inactivo.");
            }

            if (!_odontologoServicio.ExisteOdontologo(id))
            {
                return NotFound($"No se encontró el odontólogo con el id: {id}");
            }

            if (!estadoDto.Activo.Value && _odontologoServicio.TieneCitasFuturas(id))
            {
                return BadRequest("No se puede desactivar un odontólogo con citas futuras no canceladas.");
            }

            try
            {
                var actualizado = _odontologoServicio.CambiarEstado(id, estadoDto.Activo.Value);
                return Ok(actualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al cambiar el estado del odontólogo.");
            }
        }
    }
}
