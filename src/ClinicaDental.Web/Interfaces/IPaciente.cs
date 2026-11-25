using ClinicaDental.Contrato.DTOs.Paciente;
using ClinicaDental.Contrato.Respuestas.Paciente;

namespace ClinicaDental.Web.Interfaces
{
    public interface IPaciente
    {
        IEnumerable<PacienteRespuesta> ObtenerTodos();
        PacienteRespuesta? ObtenerPorId(Guid id);
        HistorialPacienteRespuesta? ObtenerHistorial(Guid id);
        PacienteRespuesta Registrar(PacienteDto pacienteDto);
        PacienteRespuesta Actualizar(Guid id, PacienteDto pacienteDto);
        void Eliminar(Guid id);

        bool DatosObligatoriosValidos(string nombres, string apellidos, string ci, string telefono, DateOnly fechaNacimiento);
        bool FechaNacimientoValida(DateOnly fechaNacimiento);
        bool ExistePaciente(Guid id);
        bool CiRepetido(string ci, Guid? excluirId = null);
        bool TieneCitas(Guid idPaciente);
        bool TieneTratamientos(Guid idPaciente);
    }
}
