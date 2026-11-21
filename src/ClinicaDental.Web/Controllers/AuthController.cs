using System.Security.Claims;
using ClinicaDental.Web.DTOs.Auth;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClinicaDental.Web.Controllers
{
    /// <summary>
    /// Inicio de sesión (público) y gestión de usuarios.
    /// El resto de controladores exige token JWT (ver Program.cs: MapControllers().RequireAuthorization()).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _authServicio;

        public AuthController(IAuth authServicio)
        {
            _authServicio = authServicio;
        }

        private Guid IdUsuarioActual => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login", Name = "IniciarSesion")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (resultado, respuesta) = _authServicio.IniciarSesion(dto);

            return resultado switch
            {
                ResultadoLogin.Exito => Ok(respuesta),
                ResultadoLogin.Bloqueado => StatusCode(StatusCodes.Status423Locked,
                    "La cuenta está bloqueada temporalmente por varios intentos fallidos. Intenta nuevamente en 15 minutos."),
                ResultadoLogin.Inactivo => StatusCode(StatusCodes.Status403Forbidden,
                    "La cuenta está desactivada. Comunícate con el administrador."),
                _ => Unauthorized("Usuario o contraseña incorrectos.")
            };
        }

        [Authorize]
        [HttpGet("yo", Name = "UsuarioActual")]
        public IActionResult Yo()
        {
            var usuario = _authServicio.ObtenerUsuario(IdUsuarioActual);
            return usuario == null ? Unauthorized() : Ok(usuario);
        }

        [Authorize]
        [HttpPut("contrasena", Name = "CambiarContrasena")]
        public IActionResult CambiarContrasena([FromBody] CambiarContrasenaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authServicio.ContrasenaCorrecta(IdUsuarioActual, dto.ContrasenaActual))
            {
                return BadRequest("La contraseña actual no es correcta.");
            }

            if (!_authServicio.ContrasenaSegura(dto.ContrasenaNueva))
            {
                return BadRequest("La nueva contraseña debe tener al menos 8 caracteres, con mayúscula, minúscula, número y símbolo.");
            }

            if (dto.ContrasenaNueva == dto.ContrasenaActual)
            {
                return BadRequest("La nueva contraseña debe ser distinta de la actual.");
            }

            try
            {
                _authServicio.CambiarContrasena(IdUsuarioActual, dto.ContrasenaNueva);
                return Ok("Contraseña actualizada correctamente.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al cambiar la contraseña.");
            }
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpGet("usuarios", Name = "ListarUsuarios")]
        public IActionResult GetUsuarios()
        {
            return Ok(_authServicio.ObtenerUsuarios());
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpPost("usuarios", Name = "RegistrarUsuario")]
        public IActionResult PostUsuario([FromBody] RegistrarUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authServicio.RolValido(dto.Rol))
            {
                return BadRequest($"El rol no es válido ({string.Join(", ", Roles.Todos)}).");
            }

            if (!_authServicio.ContrasenaSegura(dto.Contrasena))
            {
                return BadRequest("La contraseña debe tener al menos 8 caracteres, con mayúscula, minúscula, número y símbolo.");
            }

            if (_authServicio.NombreUsuarioRepetido(dto.NombreUsuario))
            {
                return BadRequest("El nombre de usuario ya está registrado.");
            }

            try
            {
                var nuevo = _authServicio.Registrar(dto);
                return CreatedAtAction(nameof(Yo), new { }, nuevo);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al registrar el usuario.");
            }
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpPut("usuarios/{id:guid}/estado", Name = "CambiarEstadoUsuario")]
        public IActionResult CambiarEstado(Guid id, [FromBody] CambiarEstadoUsuarioDto dto)
        {
            if (!ModelState.IsValid || !dto.Activo.HasValue)
            {
                return BadRequest("Debe indicar si el usuario queda activo o inactivo.");
            }

            if (!_authServicio.ExisteUsuario(id))
            {
                return NotFound($"No se encontró el usuario con el id: {id}");
            }

            if (!dto.Activo.Value && id == IdUsuarioActual)
            {
                return BadRequest("No puedes desactivar tu propia cuenta.");
            }

            if (!dto.Activo.Value && _authServicio.EsUltimoAdministradorActivo(id))
            {
                return BadRequest("Debe quedar al menos un administrador activo.");
            }

            try
            {
                return Ok(_authServicio.CambiarEstado(id, dto.Activo.Value));
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al cambiar el estado del usuario.");
            }
        }
    }
}
