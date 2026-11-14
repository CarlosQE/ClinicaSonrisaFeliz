using ClinicaDental.Web.DTOs.Cita;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    public interface ICita
    {
        IEnumerable<Cita> ObtenerTodos();
        Cita? ObtenerPorId(Guid id);
        IEnumerable<Cita> ObtenerAgendaDiaria(DateOnly fecha, Guid? idOdontologo = null, bool incluirCanceladas = false);
        Cita agendar(CitaDto cita);
        Cita reprogramar(Guid id, DateTime nuevaFechaHora, Guid? idNuevoOdontologo);
        void cancelar(Guid id, string motivo);
        bool ExistePaciente(Guid idPaciente);
        bool ExisteOdontologo(Guid idOdontologo);
        bool OdontologoActivo(Guid idOdontologo);
        bool DuracionValida(int minutos);
        bool FechaFutura(DateTime fechaHora);
        bool DentroDeHorario(Guid idOdontologo, DateTime fechaHora, int duracionMinutos);
        bool OdontologoDisponible(Guid idOdontologo, DateTime fechaHora, int duracionMinutos, Guid? excluirIdCita = null);
        bool PacienteDisponible(Guid idPaciente, DateTime fechaHora, int duracionMinutos, Guid? excluirIdCita = null);
    }
}
