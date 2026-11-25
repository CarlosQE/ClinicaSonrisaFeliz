using ClinicaDental.Contrato.DTOs.HorarioOdontologo;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;

namespace ClinicaDental.Web.Interfaces
{
    public interface IHorarioOdontologo
    {
        IEnumerable<HorarioRespuesta> ObtenerPorOdontologo(Guid idOdontologo);
        HorarioRespuesta? ObtenerPorId(Guid id);
        HorarioRespuesta Registrar(Guid idOdontologo, HorarioOdontologoDto horarioDto);
        HorarioRespuesta Actualizar(Guid id, HorarioOdontologoDto horarioDto);
        void Eliminar(Guid id);

        bool Existe(Guid id);
        bool ExisteOdontologo(Guid idOdontologo);
        bool DiaSemanaValido(DayOfWeek diaSemana);
        bool RangoValido(TimeOnly horaInicio, TimeOnly horaFin);
        bool SeSolapa(Guid idOdontologo, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid? excluirIdHorario = null);
        bool TieneCitasFuturas(Guid idHorario);
    }
}
