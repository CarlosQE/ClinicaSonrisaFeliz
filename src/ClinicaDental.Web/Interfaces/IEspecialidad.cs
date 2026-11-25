using ClinicaDental.Contrato.DTOs.Especialidad;
using ClinicaDental.Contrato.Respuestas.Especialidad;

namespace ClinicaDental.Web.Interfaces
{
    public interface IEspecialidad
    {
        IEnumerable<EspecialidadRespuesta> ObtenerTodas();
        EspecialidadRespuesta? ObtenerPorId(Guid id);
        EspecialidadRespuesta Registrar(EspecialidadDto especialidadDto);
        EspecialidadRespuesta Actualizar(Guid id, EspecialidadDto especialidadDto);
        void Eliminar(Guid id);

        bool NombreValido(string nombre);
        bool DescripcionValida(string? descripcion);
        bool NombreRepetido(string nombre, Guid? excluirId = null);
        bool TieneOdontologos(Guid idEspecialidad);
    }
}
