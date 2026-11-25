using System.Security.Claims;
using ClinicaDental.Contrato.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>
    /// Mantiene la sesión del usuario en el cliente Blazor.
    /// - El token JWT vive en memoria del servidor (un objeto por conexión/circuito), nunca en JavaScript.
    /// - Para sobrevivir a un F5 se guarda en ProtectedSessionStorage: cifrado y firmado con Data Protection,
    ///   por lo que el navegador no puede leerlo ni alterarlo, y se borra al cerrar la pestaña.
    /// - Si el token expira, la sesión se considera cerrada.
    /// </summary>
    public class ProveedorAutenticacion : AuthenticationStateProvider
    {
        private const string ClaveSesion = "sesion-sonrisa-feliz";
        private static readonly AuthenticationState Anonimo = new(new ClaimsPrincipal(new ClaimsIdentity()));

        private readonly ProtectedSessionStorage _almacen;
        private readonly TimeProvider _reloj;
        private LoginRespuestaDto? _sesion;
        private bool _restaurada;

        public ProveedorAutenticacion(ProtectedSessionStorage almacen, TimeProvider reloj)
        {
            _almacen = almacen;
            _reloj = reloj;
        }

        /// <summary>Token vigente para llamar a la API, o null si no hay sesión.</summary>
        public string? Token => SesionVigente ? _sesion!.Token : null;

        public LoginRespuestaDto? Sesion => SesionVigente ? _sesion : null;

        private bool SesionVigente => _sesion is not null && _sesion.Expira > _reloj.GetUtcNow().UtcDateTime;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_restaurada)
            {
                _restaurada = true;
                try
                {
                    var guardada = await _almacen.GetAsync<LoginRespuestaDto>(ClaveSesion);
                    _sesion = guardada.Success ? guardada.Value : null;
                }
                catch
                {
                    // Datos ilegibles (otra clave de cifrado, manipulación, etc.): se descartan.
                    _sesion = null;
                }
            }

            if (!SesionVigente) return Anonimo;

            var identidad = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, _sesion!.NombreUsuario),
                new Claim(ClaimTypes.GivenName, _sesion.NombreCompleto),
                new Claim(ClaimTypes.Role, _sesion.Rol)
            ], authenticationType: "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identidad));
        }

        public async Task IniciarSesionAsync(LoginRespuestaDto sesion)
        {
            _sesion = sesion;
            _restaurada = true;
            await _almacen.SetAsync(ClaveSesion, sesion);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task CerrarSesionAsync()
        {
            _sesion = null;
            _restaurada = true;
            try { await _almacen.DeleteAsync(ClaveSesion); } catch { /* circuito cerrándose */ }
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}
