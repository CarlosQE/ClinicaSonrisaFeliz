using ClinicaDental.Contrato.DTOs.Auth;
using ClinicaDental.Contrato.Respuestas.Auth;

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

        IEnumerable<UsuarioRespuesta> ObtenerUsuarios();
        UsuarioRespuesta? ObtenerUsuario(Guid id);
        UsuarioRespuesta Registrar(RegistrarUsuarioDto dto);
        UsuarioRespuesta CambiarEstado(Guid id, bool activo);
        void CambiarContrasena(Guid id, string contrasenaNueva);

        bool ExisteUsuario(Guid id);
        bool NombreUsuarioRepetido(string nombreUsuario);
        bool NombreUsuarioValido(string nombreUsuario);
        bool NombreCompletoValido(string nombreCompleto);
        bool RolValido(string rol);
        bool ContrasenaSegura(string contrasena);
        bool ContrasenaCorrecta(Guid id, string contrasena);
        bool EsUltimoAdministradorActivo(Guid id);
    }
}
