using ClinicaDental.Contrato.DTOs.Odontologo;
using ClinicaDental.Contrato.Respuestas.Odontologo;

namespace ClinicaDental.Web.Interfaces
{
    public interface IOdontologo
    {
        IEnumerable<OdontologoRespuesta> ObtenerTodos(bool? activo = null, Guid? idEspecialidad = null);
        OdontologoRespuesta? ObtenerPorId(Guid id);
        OdontologoRespuesta Registrar(OdontologoDto odontologoDto);
        OdontologoRespuesta Actualizar(Guid id, OdontologoDto odontologoDto);
        OdontologoRespuesta CambiarEstado(Guid id, bool activo);

        bool DatosObligatoriosValidos(string nombres, string apellidos, string matricula, string telefono);
        bool NombreValido(string nombre);
        bool MatriculaValida(string matricula);
        bool TelefonoValido(string telefono);
        bool EmailValido(string? email);
        bool ExisteOdontologo(Guid id);
        bool MatriculaRepetida(string matricula, Guid? excluirId = null);
        bool ExisteEspecialidad(Guid idEspecialidad);
        bool TieneCitasFuturas(Guid idOdontologo);
    }
}
