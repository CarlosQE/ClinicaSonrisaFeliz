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

        private static IEnumerable<CitaRespuesta> Proyectar(IQueryable<Cita> consulta)
        {
            return consulta
                .Select(c => new
                {
                    c.Id,
                    c.IdPaciente,
                    Paciente = new
                    {
                        c.Paciente!.Id,
                        c.Paciente!.Nombres,
                        c.Paciente!.Apellidos,
                        c.Paciente!.CI,
                        c.Paciente!.FechaNacimiento,
                        c.Paciente!.Telefono,
                        c.Paciente!.Email,
                        c.Paciente!.Direccion,
                        c.Paciente!.FechaRegistro
                    },
                    c.IdOdontologo,
                    Odontologo = new
                    {
                        c.Odontologo!.Id,
                        c.Odontologo!.Nombres,
                        c.Odontologo!.Apellidos,
                        c.Odontologo!.Matricula,
                        c.Odontologo!.Telefono,
                        c.Odontologo!.Email,
                        c.Odontologo!.IdEspecialidad,
                        c.Odontologo!.Activo,
                        Especialidad = new
                        {
                            c.Odontologo!.Especialidad!.Id,
                            c.Odontologo!.Especialidad!.Nombre,
                            c.Odontologo!.Especialidad!.Descripcion
                        }
                    },
                    c.Fecha,
                    c.Hora,
                    c.DuracionMinutos,
                    c.Motivo,
                    c.Estado,
                    c.MotivoCancelacion,
                    c.FechaRegistro
                })
                .ToList()
                .Select(c => new CitaRespuesta
                {
                    Id = c.Id,
                    IdPaciente = c.IdPaciente,
                    Paciente = new PacienteRespuesta
                    {
                        Id = c.Paciente.Id,
                        Nombres = c.Paciente.Nombres,
                        Apellidos = c.Paciente.Apellidos,
                        CI = c.Paciente.CI,
                        FechaNacimiento = c.Paciente.FechaNacimiento,
                        Telefono = c.Paciente.Telefono,
                        Email = c.Paciente.Email,
                        Direccion = c.Paciente.Direccion,
                        FechaRegistro = c.Paciente.FechaRegistro,
                        NombreCompleto = c.Paciente.Nombres + " " + c.Paciente.Apellidos
                    },
                    IdOdontologo = c.IdOdontologo,
                    Odontologo = new OdontologoRespuesta
                    {
                        Id = c.Odontologo.Id,
                        Nombres = c.Odontologo.Nombres,
                        Apellidos = c.Odontologo.Apellidos,
                        Matricula = c.Odontologo.Matricula,
                        Telefono = c.Odontologo.Telefono,
                        Email = c.Odontologo.Email,
                        IdEspecialidad = c.Odontologo.IdEspecialidad,
                        Activo = c.Odontologo.Activo,
                        NombreCompleto = "Dr(a). " + c.Odontologo.Nombres + " " + c.Odontologo.Apellidos,
                        Especialidad = new EspecialidadRespuesta
                        {
                            Id = c.Odontologo.Especialidad.Id,
                            Nombre = c.Odontologo.Especialidad.Nombre,
                            Descripcion = c.Odontologo.Especialidad.Descripcion
                        },
                        Horarios = new List<HorarioRespuesta>()
                    },
                    Fecha = c.Fecha,
                    Hora = c.Hora,
                    DuracionMinutos = c.DuracionMinutos,
                    Motivo = c.Motivo,
                    Estado = c.Estado,
                    MotivoCancelacion = c.MotivoCancelacion,
                    FechaRegistro = c.FechaRegistro,
                    HoraFin = c.Hora.AddMinutes(c.DuracionMinutos),
                    EstaVigente = c.Estado is EstadoCita.Programada or EstadoCita.Reprogramada
                });
        }

        public IEnumerable<CitaRespuesta> ObtenerTodos()
        {
            return Proyectar(_context.citas
                .AsNoTracking()
                .OrderBy(c => c.Fecha).ThenBy(c => c.Hora));
        }

        public CitaRespuesta? ObtenerPorId(Guid id)
        {
            return Proyectar(_context.citas
                .AsNoTracking()
                .Where(c => c.Id == id)).FirstOrDefault();
        }

        public IEnumerable<CitaRespuesta> ObtenerAgendaDiaria(DateOnly fecha, Guid? idOdontologo = null, bool incluirCanceladas = false)
        {
            var query = _context.citas
                .AsNoTracking()
                .Where(c => c.Fecha == fecha);

            if (idOdontologo.HasValue)
            {
                query = query.Where(c => c.IdOdontologo == idOdontologo.Value);
            }

            if (!incluirCanceladas)
            {
                query = query.Where(c => c.Estado != EstadoCita.Cancelada);
            }

            return Proyectar(query.OrderBy(c => c.Hora));
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
            return minutos >= DuracionMinima && minutos <= DuracionMaxima && minutos % 15 == 0;
        }

        public bool FechaFutura(DateOnly fecha, TimeOnly hora)
        {
            var fechaHora = fecha.ToDateTime(hora);
            return fechaHora > Ahora;
        }

        public bool FechaDentroDeRango(DateOnly fecha, TimeOnly hora)
        {
            var fechaHora = fecha.ToDateTime(hora);
            var ahora = Ahora;
            if (fechaHora <= ahora) return false;
            var max = ahora.AddDays(365);
            return fechaHora <= max;
        }

        public bool MotivoValido(string? motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo)) return false;
            var n = motivo.Trim();
            return n.Length >= 3 && n.Length <= 200
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @".*\p{L}.*");
        }

        public bool DentroDeHorario(Guid idOdontologo, DateOnly fecha, TimeOnly hora, int duracionMinutos)
        {
            var inicioPermitido = new TimeOnly(6, 0);
            var finPermitido = new TimeOnly(22, 0);
            var horaFin = hora.AddMinutes(duracionMinutos);

            if (hora < inicioPermitido || horaFin > finPermitido || horaFin < hora) return false;

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

    }
}
