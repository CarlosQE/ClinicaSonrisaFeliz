using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Auth;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClinicaDental.Web.Services
{
    /// <summary>
    /// Autenticación y gestión de usuarios.
    /// Seguridad aplicada: contraseñas con PBKDF2 + sal, bloqueo temporal tras intentos fallidos,
    /// mensaje genérico ante credenciales inválidas (no revela si el usuario existe),
    /// tokens JWT firmados con HMAC-SHA256 y con expiración.
    /// </summary>
    public class AuthServicio : IAuth
    {
        public const int MaximoIntentos = 5;
        public static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(15);

        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;
        private readonly IConfiguration _config;

        public AuthServicio(BaseDatos context, TimeProvider reloj, IConfiguration config)
        {
            _context = context;
            _reloj = reloj;
            _config = config;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public (ResultadoLogin Resultado, LoginRespuestaDto? Respuesta) IniciarSesion(LoginDto dto)
        {
            var nombre = dto.NombreUsuario.Trim().ToLower();
            var usuario = _context.usuarios.FirstOrDefault(u => u.NombreUsuario == nombre);

            if (usuario == null)
            {
                // Se calcula un hash igualmente para que el tiempo de respuesta no revele si el usuario existe.
                HasherContrasena.Hashear(dto.Contrasena);
                return (ResultadoLogin.CredencialesInvalidas, null);
            }

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > Ahora)
            {
                return (ResultadoLogin.Bloqueado, null);
            }

            if (!HasherContrasena.Verificar(dto.Contrasena, usuario.ContrasenaHash))
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= MaximoIntentos)
                {
                    usuario.BloqueadoHasta = Ahora.Add(DuracionBloqueo);
                    usuario.IntentosFallidos = 0;
                }
                _context.SaveChanges();
                return (ResultadoLogin.CredencialesInvalidas, null);
            }

            if (!usuario.Activo)
            {
                return (ResultadoLogin.Inactivo, null);
            }

            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;
            usuario.UltimoAcceso = Ahora;
            _context.SaveChanges();

            return (ResultadoLogin.Exito, GenerarToken(usuario));
        }

        private LoginRespuestaDto GenerarToken(Usuario usuario)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Clave"]!));
            var horas = _config.GetValue("Jwt:DuracionHoras", 8);
            var expira = _reloj.GetUtcNow().UtcDateTime.AddHours(horas);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("nombre_completo", usuario.NombreCompleto)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Emisor"],
                audience: _config["Jwt:Audiencia"],
                claims: claims,
                expires: expira,
                signingCredentials: new SigningCredentials(clave, SecurityAlgorithms.HmacSha256));

            return new LoginRespuestaDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                NombreUsuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol,
                Expira = expira
            };
        }

        public IEnumerable<UsuarioRespuestaDto> ObtenerUsuarios()
        {
            return _context.usuarios
                .AsNoTracking()
                .OrderBy(u => u.NombreCompleto)
                .ToList()
                .Select(ARespuesta)
                .ToList();
        }

        public UsuarioRespuestaDto? ObtenerUsuario(Guid id)
        {
            var usuario = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
            return usuario == null ? null : ARespuesta(usuario);
        }

        public UsuarioRespuestaDto Registrar(RegistrarUsuarioDto dto)
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = dto.NombreUsuario.Trim().ToLower(),
                NombreCompleto = dto.NombreCompleto.Trim(),
                ContrasenaHash = HasherContrasena.Hashear(dto.Contrasena),
                Rol = dto.Rol,
                Activo = true,
                FechaRegistro = Ahora
            };
            _context.usuarios.Add(usuario);
            _context.SaveChanges();
            return ARespuesta(usuario);
        }

        public UsuarioRespuestaDto CambiarEstado(Guid id, bool activo)
        {
            var usuario = _context.usuarios.First(u => u.Id == id);
            usuario.Activo = activo;
            if (activo)
            {
                usuario.BloqueadoHasta = null;
                usuario.IntentosFallidos = 0;
            }
            _context.SaveChanges();
            return ARespuesta(usuario);
        }

        public void CambiarContrasena(Guid id, string contrasenaNueva)
        {
            var usuario = _context.usuarios.First(u => u.Id == id);
            usuario.ContrasenaHash = HasherContrasena.Hashear(contrasenaNueva);
            _context.SaveChanges();
        }

        public bool ExisteUsuario(Guid id) => _context.usuarios.Any(u => u.Id == id);

        public bool NombreUsuarioRepetido(string nombreUsuario)
        {
            var nombre = nombreUsuario.Trim().ToLower();
            return _context.usuarios.Any(u => u.NombreUsuario == nombre);
        }

        public bool RolValido(string rol) => Roles.Todos.Contains(rol);

        /// <summary>Mínimo 8 caracteres, con mayúscula, minúscula, número y símbolo.</summary>
        public bool ContrasenaSegura(string contrasena)
        {
            return contrasena.Length >= 8
                && contrasena.Any(char.IsUpper)
                && contrasena.Any(char.IsLower)
                && contrasena.Any(char.IsDigit)
                && contrasena.Any(c => !char.IsLetterOrDigit(c));
        }

        public bool ContrasenaCorrecta(Guid id, string contrasena)
        {
            var usuario = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
            return usuario != null && HasherContrasena.Verificar(contrasena, usuario.ContrasenaHash);
        }

        public bool EsUltimoAdministradorActivo(Guid id)
        {
            var usuario = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (usuario == null || usuario.Rol != Roles.Administrador || !usuario.Activo) return false;
            return !_context.usuarios.Any(u => u.Id != id && u.Rol == Roles.Administrador && u.Activo);
        }

        private UsuarioRespuestaDto ARespuesta(Usuario u) => new()
        {
            Id = u.Id,
            NombreUsuario = u.NombreUsuario,
            NombreCompleto = u.NombreCompleto,
            Rol = u.Rol,
            Activo = u.Activo,
            Bloqueado = u.BloqueadoHasta.HasValue && u.BloqueadoHasta > Ahora,
            FechaRegistro = u.FechaRegistro,
            UltimoAcceso = u.UltimoAcceso
        };
    }
}
