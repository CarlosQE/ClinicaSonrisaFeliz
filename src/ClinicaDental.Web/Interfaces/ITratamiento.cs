using ClinicaDental.Web.DTOs.Tratamiento;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    public interface ITratamiento
    {
        IEnumerable<Tratamiento> ObtenerTodos(Guid? idPaciente = null, TipoTratamiento? tipo = null, Guid? idOdontologo = null);
        Tratamiento? ObtenerPorId(Guid id);
        Tratamiento Registrar(TratamientoDto tratamientoDto);
        Tratamiento ActualizarObservaciones(Guid id, string observaciones);
        void Eliminar(Guid id);

        bool ExisteTratamiento(Guid id);
        bool ExistePaciente(Guid idPaciente);
        bool ExisteOdontologo(Guid idOdontologo);
        bool OdontologoActivo(Guid idOdontologo);
        bool ExisteCita(Guid idCita);
        bool CitaPerteneceAPaciente(Guid idCita, Guid idPaciente);
        bool CitaNoCancelada(Guid idCita);
        bool TipoValido(TipoTratamiento tipo);
        bool CostoValido(decimal costo);
        bool FechaValida(DateOnly fecha);
        bool ObservacionesValidas(string? observaciones);
        bool TienePagos(Guid idTratamiento);
    }
}
