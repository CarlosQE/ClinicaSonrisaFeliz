using ClinicaDental.Web.DTOs.Odontologo;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    /// <summary>
    /// Parte A del módulo 2: gestión de odontólogos.
    /// Las especialidades y los horarios se gestionan en la Parte B.
    /// </summary>
    public interface IOdontologo
    {
        IEnumerable<Odontologo> ObtenerTodos(bool? activo = null, Guid? idEspecialidad = null);
        Odontologo? ObtenerPorId(Guid id);
        Odontologo Registrar(OdontologoDto odontologoDto);
        Odontologo Actualizar(Guid id, ActualizarOdontologoDto odontologoDto);
        Odontologo CambiarEstado(Guid id, bool activo);

        bool DatosObligatoriosValidos(string nombres, string apellidos, string matricula, string telefono);
        bool ExisteOdontologo(Guid id);
        bool MatriculaRepetida(string matricula, Guid? excluirId = null);
        bool ExisteEspecialidad(Guid idEspecialidad);
        bool TieneCitasFuturas(Guid idOdontologo);
    }
}
