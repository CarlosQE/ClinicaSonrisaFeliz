using ClinicaDental.Web.DTOs.Especialidad;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    /// <summary>
    /// Parte B del módulo 2: gestión de especialidades.
    /// </summary>
    public interface IEspecialidad
    {
        IEnumerable<Especialidad> ObtenerTodas();
        Especialidad? ObtenerPorId(Guid id);
        Especialidad Registrar(EspecialidadDto especialidadDto);
        Especialidad Actualizar(Guid id, EspecialidadDto especialidadDto);
        void Eliminar(Guid id);

        bool NombreValido(string nombre);
        bool NombreRepetido(string nombre, Guid? excluirId = null);
        bool TieneOdontologos(Guid idEspecialidad);
    }
}
