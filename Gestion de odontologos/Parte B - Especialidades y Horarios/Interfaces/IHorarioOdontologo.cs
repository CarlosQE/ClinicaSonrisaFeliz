using ClinicaDental.Web.DTOs.HorarioOdontologo;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Interfaces
{
    /// <summary>
    /// Parte B del módulo 2: gestión de horarios de trabajo de los odontólogos.
    /// </summary>
    public interface IHorarioOdontologo
    {
        IEnumerable<HorarioOdontologo> ObtenerPorOdontologo(Guid idOdontologo);
        HorarioOdontologo? ObtenerPorId(Guid id);
        HorarioOdontologo Registrar(Guid idOdontologo, HorarioOdontologoDto horarioDto);
        HorarioOdontologo Actualizar(Guid id, HorarioOdontologoDto horarioDto);
        void Eliminar(Guid id);

        bool Existe(Guid id);
        bool ExisteOdontologo(Guid idOdontologo);
        bool DiaSemanaValido(DayOfWeek diaSemana);
        bool RangoValido(TimeOnly horaInicio, TimeOnly horaFin);
        bool SeSolapa(Guid idOdontologo, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid? excluirIdHorario = null);
        bool TieneCitasFuturas(Guid idHorario);
    }
}
