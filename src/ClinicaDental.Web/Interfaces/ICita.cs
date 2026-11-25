using ClinicaDental.Contrato.DTOs.Cita;
using ClinicaDental.Contrato.Respuestas.Cita;

namespace ClinicaDental.Web.Interfaces
{
    public interface ICita
    {
        IEnumerable<CitaRespuesta> ObtenerTodos();
        CitaRespuesta? ObtenerPorId(Guid id);
        IEnumerable<CitaRespuesta> ObtenerAgendaDiaria(DateOnly fecha, Guid? idOdontologo = null, bool incluirCanceladas = false);
        CitaRespuesta agendar(CitaDto cita);
        CitaRespuesta reprogramar(Guid id, DateOnly nuevaFecha, TimeOnly nuevaHora, Guid? idNuevoOdontologo);
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
