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
        Cita reprogramar(Guid id, DateOnly nuevaFecha, TimeOnly nuevaHora, Guid? idNuevoOdontologo);
        void cancelar(Guid id, string motivo);
        bool ExistePaciente(Guid idPaciente);
        bool ExisteOdontologo(Guid idOdontologo);
        bool OdontologoActivo(Guid idOdontologo);
        bool DuracionValida(int minutos);
        bool FechaFutura(DateOnly fecha, TimeOnly hora);
        bool DentroDeHorario(Guid idOdontologo, DateOnly fecha, TimeOnly hora, int duracionMinutos);
        bool OdontologoDisponible(Guid idOdontologo, DateOnly fecha, TimeOnly hora, int duracionMinutos, Guid? excluirIdCita = null);
        bool PacienteDisponible(Guid idPaciente, DateOnly fecha, TimeOnly hora, int duracionMinutos, Guid? excluirIdCita = null);
    }
}
