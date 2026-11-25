using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicaDental.Contrato.DTOs.Auth;
using ClinicaDental.Contrato.Respuestas.Auth;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClinicaDental.Web.Services
{
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

        public IEnumerable<UsuarioRespuesta> ObtenerUsuarios()
        {
            return _context.usuarios
                .AsNoTracking()
                .OrderBy(u => u.NombreCompleto)
                .ToList()
                .Select(Mapear)
                .ToList();
        }

        public UsuarioRespuesta? ObtenerUsuario(Guid id)
        {
            var usuario = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
            return usuario == null ? null : Mapear(usuario);
        }

        public UsuarioRespuesta Registrar(RegistrarUsuarioDto dto)
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
            return Mapear(usuario);
        }

        public UsuarioRespuesta CambiarEstado(Guid id, bool activo)
        {
            var usuario = _context.usuarios.First(u => u.Id == id);
            usuario.Activo = activo;
            if (activo)
            {
                usuario.BloqueadoHasta = null;
                usuario.IntentosFallidos = 0;
            }
            _context.SaveChanges();
            return Mapear(usuario);
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

        public bool NombreUsuarioValido(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return false;
            var n = nombreUsuario.Trim();
            return n.Length >= 3 && n.Length <= 30
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^(?=.*[a-zA-Z])[a-zA-Z0-9._-]+$");
        }

        public bool NombreCompletoValido(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto)) return false;
            var n = nombreCompleto.Trim();
            return n.Length >= 2 && n.Length <= 50
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\p{L}+([ '\-]\p{L}+)*$");
        }

        public bool RolValido(string rol) => !string.IsNullOrWhiteSpace(rol) && Roles.Todos.Contains(rol);

        public bool ContrasenaSegura(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena)) return false;
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

        private UsuarioRespuesta Mapear(Usuario u) => new()
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
