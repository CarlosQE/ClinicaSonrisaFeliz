using ClinicaDental.Contrato.DTOs.Tratamiento;
using ClinicaDental.Contrato.Respuestas.Tratamiento;

namespace ClinicaDental.Web.Interfaces
{
    public interface ITratamiento
    {
        IEnumerable<TratamientoRespuesta> ObtenerTodos(Guid? idPaciente = null, TipoTratamiento? tipo = null, Guid? idOdontologo = null);
        TratamientoRespuesta? ObtenerPorId(Guid id);
        TratamientoRespuesta Registrar(TratamientoDto tratamientoDto);
        TratamientoRespuesta ActualizarObservaciones(Guid id, string observaciones);
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
