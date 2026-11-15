using ClinicaDental.Web.DTOs.Paciente;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    public interface IPaciente
    {
        IEnumerable<Paciente> ObtenerTodos();
        Paciente? ObtenerPorId(Guid id);
        HistorialPacienteDto? ObtenerHistorial(Guid id);
        Paciente Registrar(PacienteDto pacienteDto);
        Paciente Actualizar(Guid id, ActualizarPacienteDto pacienteDto);
        void Eliminar(Guid id);

        bool DatosObligatoriosValidos(string nombres, string apellidos, string ci, string telefono, DateOnly fechaNacimiento);
        bool FechaNacimientoValida(DateOnly fechaNacimiento);
        bool ExistePaciente(Guid id);
        bool CiRepetido(string ci, Guid? excluirId = null);
        bool TieneCitas(Guid idPaciente);
        bool TieneTratamientos(Guid idPaciente);
    }
}
