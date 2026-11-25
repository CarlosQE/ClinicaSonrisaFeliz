using ClinicaDental.Contrato.DTOs.Cita;
using ClinicaDental.Contrato.Respuestas.Cita;
using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Contrato.Respuestas.Paciente;
using ClinicaDental.Web.Data;
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

        public IEnumerable<CitaRespuesta> ObtenerTodos()
        {
            return _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .OrderBy(c => c.Fecha).ThenBy(c => c.Hora)
                .AsNoTracking()
                .ToList()
                .Select(Mapear);
        }

        public CitaRespuesta? ObtenerPorId(Guid id)
        {
            var cita = _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
            return cita == null ? null : Mapear(cita);
        }

        public IEnumerable<CitaRespuesta> ObtenerAgendaDiaria(DateOnly fecha, Guid? idOdontologo = null, bool incluirCanceladas = false)
        {
            var query = _context.citas
                .Include(c => c.Paciente)
                .Include(c => c.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .Where(c => c.Fecha == fecha);

            if (idOdontologo.HasValue)
            {
                query = query.Where(c => c.IdOdontologo == idOdontologo.Value);
            }

            if (!incluirCanceladas)
            {
                query = query.Where(c => c.Estado != EstadoCita.Cancelada);
            }

            return query
                .OrderBy(c => c.Hora)
                .AsNoTracking()
                .ToList()
                .Select(Mapear);
        }

        public CitaRespuesta agendar(CitaDto citaDto)
        {
            var cita = new Cita
            {
                Id = Guid.NewGuid(),
                IdPaciente = citaDto.IdPaciente!.Value,
                IdOdontologo = citaDto.IdOdontologo!.Value,
                Fecha = citaDto.Fecha!.Value,
                Hora = citaDto.Hora!.Value,
                DuracionMinutos = citaDto.DuracionMinutos,
                Motivo = citaDto.Motivo.Trim(),
                Estado = EstadoCita.Programada,
                FechaRegistro = Ahora
            };

            _context.citas.Add(cita);
            _context.SaveChanges();

            return ObtenerPorId(cita.Id)!;
        }

        public CitaRespuesta reprogramar(Guid id, DateOnly nuevaFecha, TimeOnly nuevaHora, Guid? idNuevoOdontologo)
        {
            var cita = _context.citas.First(c => c.Id == id);

            cita.Fecha = nuevaFecha;
            cita.Hora = nuevaHora;
            if (idNuevoOdontologo.HasValue)
            {
                cita.IdOdontologo = idNuevoOdontologo.Value;
            }
            cita.Estado = EstadoCita.Reprogramada;

            _context.SaveChanges();

            return ObtenerPorId(id)!;
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

        public bool FechaFutura(DateOnly fecha, TimeOnly hora)
        {
            var fechaHora = fecha.ToDateTime(hora);
            return fechaHora > Ahora;
        }

        public bool DentroDeHorario(Guid idOdontologo, DateOnly fecha, TimeOnly hora, int duracionMinutos)
        {
            var horaFin = hora.AddMinutes(duracionMinutos);
            var dia = fecha.DayOfWeek;

            return _context.horariosOdontologo.Any(h =>
                h.IdOdontologo == idOdontologo &&
                h.DiaSemana == dia &&
                h.HoraInicio <= hora &&
                h.HoraFin >= horaFin);
        }

        public bool OdontologoDisponible(Guid idOdontologo, DateOnly fecha, TimeOnly hora, int duracionMinutos, Guid? excluirIdCita = null)
        {
            var nuevaInicio = hora;
            var nuevaFin = hora.AddMinutes(duracionMinutos);

            var citasDelDia = _context.citas
                .Where(c => c.IdOdontologo == idOdontologo
                         && c.Fecha == fecha
                         && c.Estado != EstadoCita.Cancelada
                         && (!excluirIdCita.HasValue || c.Id != excluirIdCita.Value))
                .Select(c => new { c.Hora, c.DuracionMinutos })
                .ToList();

            return !citasDelDia.Any(c =>
            {
                var existenteInicio = c.Hora;
                var existenteFin = c.Hora.AddMinutes(c.DuracionMinutos);
                return nuevaInicio < existenteFin && nuevaFin > existenteInicio;
            });
        }

        public bool PacienteDisponible(Guid idPaciente, DateOnly fecha, TimeOnly hora, int duracionMinutos, Guid? excluirIdCita = null)
        {
            var nuevaInicio = hora;
            var nuevaFin = hora.AddMinutes(duracionMinutos);

            var citasDelDia = _context.citas
                .Where(c => c.IdPaciente == idPaciente
                         && c.Fecha == fecha
                         && c.Estado != EstadoCita.Cancelada
                         && (!excluirIdCita.HasValue || c.Id != excluirIdCita.Value))
                .Select(c => new { c.Hora, c.DuracionMinutos })
                .ToList();

            return !citasDelDia.Any(c =>
            {
                var existenteInicio = c.Hora;
                var existenteFin = c.Hora.AddMinutes(c.DuracionMinutos);
                return nuevaInicio < existenteFin && nuevaFin > existenteInicio;
            });
        }

        private static CitaRespuesta Mapear(Cita c)
        {
            return new CitaRespuesta
            {
                Id = c.Id,
                IdPaciente = c.IdPaciente,
                Paciente = c.Paciente == null ? null : MapearPaciente(c.Paciente),
                IdOdontologo = c.IdOdontologo,
                Odontologo = c.Odontologo == null ? null : MapearOdontologo(c.Odontologo),
                Fecha = c.Fecha,
                Hora = c.Hora,
                DuracionMinutos = c.DuracionMinutos,
                Motivo = c.Motivo,
                Estado = c.Estado,
                MotivoCancelacion = c.MotivoCancelacion,
                FechaRegistro = c.FechaRegistro,
                HoraFin = c.HoraFin,
                EstaVigente = c.EstaVigente
            };
        }

        private static PacienteRespuesta MapearPaciente(Paciente p)
        {
            return new PacienteRespuesta
            {
                Id = p.Id,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                CI = p.CI,
                FechaNacimiento = p.FechaNacimiento,
                Telefono = p.Telefono,
                Email = p.Email,
                Direccion = p.Direccion,
                FechaRegistro = p.FechaRegistro,
                NombreCompleto = p.NombreCompleto
            };
        }

        private static OdontologoRespuesta MapearOdontologo(Odontologo o)
        {
            return new OdontologoRespuesta
            {
                Id = o.Id,
                Nombres = o.Nombres,
                Apellidos = o.Apellidos,
                Matricula = o.Matricula,
                Telefono = o.Telefono,
                Email = o.Email,
                IdEspecialidad = o.IdEspecialidad,
                Activo = o.Activo,
                NombreCompleto = o.NombreCompleto,
                Especialidad = o.Especialidad == null ? null : new EspecialidadRespuesta
                {
                    Id = o.Especialidad.Id,
                    Nombre = o.Especialidad.Nombre,
                    Descripcion = o.Especialidad.Descripcion
                },
                Horarios = new List<HorarioRespuesta>()
            };
        }
    }
}
