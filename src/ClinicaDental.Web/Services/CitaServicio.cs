using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Cita;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    public class CitaServicio : ICita
    {
        public const int DuracionMinima = 15;
        public const int DuracionMaxima = 240;

        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public CitaServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<Cita> ObtenerTodos()
        {
            return _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                .OrderBy(c => c.FechaHora)
                .ToList();
        }

        public Cita? ObtenerPorId(Guid id)
        {
            return _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                .FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Cita> ObtenerAgendaDiaria(DateOnly fecha, Guid? idOdontologo = null, bool incluirCanceladas = false)
        {
            var desde = fecha.ToDateTime(TimeOnly.MinValue);
            var hasta = desde.AddDays(1);

            var query = _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                .Where(c => c.FechaHora >= desde && c.FechaHora < hasta);

            if (idOdontologo.HasValue)
            {
                query = query.Where(c => c.IdOdontologo == idOdontologo.Value);
            }

            if (!incluirCanceladas)
            {
                query = query.Where(c => c.Estado != EstadoCita.Cancelada);
            }

            return query.OrderBy(c => c.FechaHora).ToList();
        }

        public Cita agendar(CitaDto citaDto)
        {
            var cita = new Cita
            {
                Id = Guid.NewGuid(),
                IdPaciente = citaDto.IdPaciente,
                IdOdontologo = citaDto.IdOdontologo,
                FechaHora = citaDto.FechaHora,
                DuracionMinutos = citaDto.DuracionMinutos,
                Motivo = citaDto.Motivo.Trim(),
                Estado = EstadoCita.Programada,
                FechaRegistro = Ahora
            };

            _context.citas.Add(cita);
            _context.SaveChanges();

            return cita;
        }

        public Cita reprogramar(Guid id, DateTime nuevaFechaHora, Guid? idNuevoOdontologo)
        {
            var cita = _context.citas.First(c => c.Id == id);

            cita.FechaHora = nuevaFechaHora;
            cita.IdOdontologo = idNuevoOdontologo ?? cita.IdOdontologo;
            cita.Estado = EstadoCita.Reprogramada;

            _context.SaveChanges();

            return cita;
        }

        public void cancelar(Guid id, string motivo)
        {
            var cita = _context.citas.First(c => c.Id == id);

            cita.Estado = EstadoCita.Cancelada;
            cita.MotivoCancelacion = motivo.Trim();

            _context.SaveChanges();
        }

        public bool ExistePaciente(Guid idPaciente)
        {
            return _context.pacientes.Any(p => p.Id == idPaciente);
        }

        public bool ExisteOdontologo(Guid idOdontologo)
        {
            return _context.odontologos.Any(o => o.Id == idOdontologo);
        }

        public bool OdontologoActivo(Guid idOdontologo)
        {
            return _context.odontologos.Any(o => o.Id == idOdontologo && o.Activo);
        }

        public bool DuracionValida(int minutos)
        {
            return minutos >= DuracionMinima && minutos <= DuracionMaxima;
        }

        public bool FechaFutura(DateTime fechaHora)
        {
            return fechaHora > Ahora;
        }

        public bool DentroDeHorario(Guid idOdontologo, DateTime fechaHora, int duracionMinutos)
        {
            var fin = fechaHora.AddMinutes(duracionMinutos);

            if (fin.Date != fechaHora.Date)
            {
                return false;
            }

            var dia = fechaHora.DayOfWeek;
            var horaInicio = TimeOnly.FromDateTime(fechaHora);
            var horaFin = TimeOnly.FromDateTime(fin);

            return _context.horariosOdontologo.Any(h =>
                h.IdOdontologo == idOdontologo &&
                h.DiaSemana == dia &&
                h.HoraInicio <= horaInicio &&
                horaFin <= h.HoraFin);
        }

        public bool OdontologoDisponible(Guid idOdontologo, DateTime fechaHora, int duracionMinutos, Guid? excluirIdCita = null)
        {
            return !HayChoque(c => c.IdOdontologo == idOdontologo, fechaHora, duracionMinutos, excluirIdCita);
        }

        public bool PacienteDisponible(Guid idPaciente, DateTime fechaHora, int duracionMinutos, Guid? excluirIdCita = null)
        {
            return !HayChoque(c => c.IdPaciente == idPaciente, fechaHora, duracionMinutos, excluirIdCita);
        }

        private bool HayChoque(System.Linq.Expressions.Expression<Func<Cita, bool>> filtro, DateTime inicio, int duracionMinutos, Guid? excluirIdCita)
        {
            var fin = inicio.AddMinutes(duracionMinutos);
            var desde = inicio.Date;
            var hasta = desde.AddDays(1);

            var query = _context.citas.AsNoTracking()
                .Where(filtro)
                .Where(c => c.FechaHora >= desde && c.FechaHora < hasta && c.Estado != EstadoCita.Cancelada);

            if (excluirIdCita.HasValue)
            {
                query = query.Where(c => c.Id != excluirIdCita.Value);
            }

            return query.ToList().Any(c => inicio < c.FechaHoraFin && c.FechaHora < fin);
        }
    }
}
