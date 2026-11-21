using ClinicaDental.Web.DTOs.Auth;

namespace ClinicaDental.Web.Interfaces
{
    public enum ResultadoLogin
    {
        Exito,
        CredencialesInvalidas,
        Bloqueado,
        Inactivo
    }

    public interface IAuth
    {
        (ResultadoLogin Resultado, LoginRespuestaDto? Respuesta) IniciarSesion(LoginDto dto);

        IEnumerable<UsuarioRespuestaDto> ObtenerUsuarios();
        UsuarioRespuestaDto? ObtenerUsuario(Guid id);
        UsuarioRespuestaDto Registrar(RegistrarUsuarioDto dto);
        UsuarioRespuestaDto CambiarEstado(Guid id, bool activo);
        void CambiarContrasena(Guid id, string contrasenaNueva);

        bool ExisteUsuario(Guid id);
        bool NombreUsuarioRepetido(string nombreUsuario);
        bool RolValido(string rol);
        bool ContrasenaSegura(string contrasena);
        bool ContrasenaCorrecta(Guid id, string contrasena);
        bool EsUltimoAdministradorActivo(Guid id);
    }
}
