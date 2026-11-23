using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.HorarioOdontologo;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    /// <summary>
    /// Parte B del módulo 2: gestión de horarios de trabajo de los odontólogos.
    /// </summary>
    public class HorarioOdontologoServicio : IHorarioOdontologo
    {
        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public HorarioOdontologoServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<HorarioOdontologo> ObtenerPorOdontologo(Guid idOdontologo)
        {
            return _context.horariosOdontologo
                .AsNoTracking()
                .Where(h => h.IdOdontologo == idOdontologo)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToList();
        }

        public HorarioOdontologo? ObtenerPorId(Guid id)
        {
            return _context.horariosOdontologo
                .Include(h => h.Odontologo)
                .AsNoTracking()
                .FirstOrDefault(h => h.Id == id);
        }

        public HorarioOdontologo Registrar(Guid idOdontologo, HorarioOdontologoDto horarioDto)
        {
            var horario = new HorarioOdontologo
            {
                Id = Guid.NewGuid(),
                IdOdontologo = idOdontologo,
                DiaSemana = horarioDto.DiaSemana,
                HoraInicio = horarioDto.HoraInicio,
                HoraFin = horarioDto.HoraFin
            };

            _context.horariosOdontologo.Add(horario);
            _context.SaveChanges();

            return horario;
        }

        public HorarioOdontologo Actualizar(Guid id, HorarioOdontologoDto horarioDto)
        {
            var horario = _context.horariosOdontologo.First(h => h.Id == id);

            horario.DiaSemana = horarioDto.DiaSemana;
            horario.HoraInicio = horarioDto.HoraInicio;
            horario.HoraFin = horarioDto.HoraFin;

            _context.SaveChanges();

            return horario;
        }

        public void Eliminar(Guid id)
        {
            var horario = _context.horariosOdontologo.First(h => h.Id == id);
            _context.horariosOdontologo.Remove(horario);
            _context.SaveChanges();
        }

        public bool Existe(Guid id)
        {
            return _context.horariosOdontologo.Any(h => h.Id == id);
        }

        public bool ExisteOdontologo(Guid idOdontologo)
        {
            return _context.odontologos.Any(o => o.Id == idOdontologo);
        }

        public bool DiaSemanaValido(DayOfWeek diaSemana)
        {
            return Enum.IsDefined(typeof(DayOfWeek), diaSemana);
        }

        public bool RangoValido(TimeOnly horaInicio, TimeOnly horaFin)
        {
            return horaFin > horaInicio;
        }

        public bool SeSolapa(Guid idOdontologo, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid? excluirIdHorario = null)
        {
            var query = _context.horariosOdontologo
                .AsNoTracking()
                .Where(h => h.IdOdontologo == idOdontologo && h.DiaSemana == diaSemana);

            if (excluirIdHorario.HasValue)
            {
                query = query.Where(h => h.Id != excluirIdHorario.Value);
            }

            return query.Any(h => horaInicio < h.HoraFin && h.HoraInicio < horaFin);
        }

        public bool TieneCitasFuturas(Guid idHorario)
        {
            var horario = _context.horariosOdontologo
                .AsNoTracking()
                .FirstOrDefault(h => h.Id == idHorario);

            if (horario == null)
            {
                return false;
            }

            var ahora = Ahora;
            var hoy = DateOnly.FromDateTime(ahora);
            var horaActual = TimeOnly.FromDateTime(ahora);

            var citasFuturas = _context.citas
                .AsNoTracking()
                .Where(c => c.IdOdontologo == horario.IdOdontologo &&
                            c.Estado != EstadoCita.Cancelada &&
                            (c.Fecha > hoy || (c.Fecha == hoy && c.Hora > horaActual)))
                .ToList();

            return citasFuturas.Any(c =>
                c.Fecha.DayOfWeek == horario.DiaSemana &&
                c.Hora < horario.HoraFin &&
                horario.HoraInicio < c.HoraFin);
        }
    }
}
