using ClinicaDental.Contrato.DTOs.Tratamiento;
using ClinicaDental.Contrato.Respuestas.Cita;
using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Contrato.Respuestas.Paciente;
using ClinicaDental.Contrato.Respuestas.Tratamiento;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    public class TratamientoServicio : ITratamiento
    {
        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public TratamientoServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<TratamientoRespuesta> ObtenerTodos(Guid? idPaciente = null, TipoTratamiento? tipo = null, Guid? idOdontologo = null)
        {
            var query = _context.tratamientos
                .Include(t => t.Paciente)
                .Include(t => t.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .Include(t => t.Cita)
                .AsNoTracking()
                .AsQueryable();

            if (idPaciente.HasValue)
            {
                query = query.Where(t => t.IdPaciente == idPaciente.Value);
            }

            if (tipo.HasValue)
            {
                query = query.Where(t => t.Tipo == tipo.Value);
            }

            if (idOdontologo.HasValue)
            {
                query = query.Where(t => t.IdOdontologo == idOdontologo.Value);
            }

            return query
                .OrderByDescending(t => t.Fecha)
                .ToList()
                .Select(Mapear);
        }

        public TratamientoRespuesta? ObtenerPorId(Guid id)
        {
            var t = _context.tratamientos
                .Include(t => t.Paciente)
                .Include(t => t.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .Include(t => t.Cita)
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);
            return t == null ? null : Mapear(t);
        }

        public TratamientoRespuesta Registrar(TratamientoDto tratamientoDto)
        {
            var idCita = tratamientoDto.IdCita == Guid.Empty ? null : tratamientoDto.IdCita;

            if (idCita.HasValue)
            {
                var cita = _context.citas.First(c => c.Id == idCita.Value);
                if (cita.EstaVigente)
                {
                    cita.Estado = EstadoCita.Atendida;
                }
            }

            var tratamiento = new Tratamiento
            {
                Id = Guid.NewGuid(),
                IdPaciente = tratamientoDto.IdPaciente!.Value,
                IdOdontologo = tratamientoDto.IdOdontologo!.Value,
                IdCita = idCita,
                Tipo = tratamientoDto.Tipo,
                Fecha = tratamientoDto.Fecha ?? DateOnly.FromDateTime(Ahora),
                Costo = tratamientoDto.Costo,
                Observaciones = string.IsNullOrWhiteSpace(tratamientoDto.Observaciones)
                    ? null
                    : tratamientoDto.Observaciones.Trim()
            };

            _context.tratamientos.Add(tratamiento);
            _context.SaveChanges();

            return ObtenerPorId(tratamiento.Id)!;
        }

        public TratamientoRespuesta ActualizarObservaciones(Guid id, string observaciones)
        {
            var tratamiento = _context.tratamientos.First(t => t.Id == id);

            tratamiento.Observaciones = observaciones.Trim();
            _context.SaveChanges();

            return ObtenerPorId(id)!;
        }

        public void Eliminar(Guid id)
        {
            var tratamiento = _context.tratamientos.First(t => t.Id == id);

            _context.tratamientos.Remove(tratamiento);
            _context.SaveChanges();
        }

        public bool ExisteTratamiento(Guid id)
        {
            return _context.tratamientos.Any(t => t.Id == id);
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

        public bool ExisteCita(Guid idCita)
        {
            return _context.citas.Any(c => c.Id == idCita);
        }

        public bool CitaPerteneceAPaciente(Guid idCita, Guid idPaciente)
        {
            return _context.citas.Any(c => c.Id == idCita && c.IdPaciente == idPaciente);
        }

        public bool CitaNoCancelada(Guid idCita)
        {
            return _context.citas.Any(c => c.Id == idCita && c.Estado != EstadoCita.Cancelada);
        }

        public bool TipoValido(TipoTratamiento tipo)
        {
            return Enum.IsDefined(tipo);
        }

        public bool CostoValido(decimal costo)
        {
            return costo > 0 && costo <= 1000000m && decimal.Round(costo, 2) == costo;
        }

        public bool FechaValida(DateOnly fecha)
        {
            return fecha != default;
        }

        public bool ObservacionesValidas(string? observaciones)
        {
            return !string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length <= 1000;
        }

        public bool TienePagos(Guid idTratamiento)
        {
            return _context.pagos.Any(p => p.IdTratamiento == idTratamiento);
        }

        private static TratamientoRespuesta Mapear(Tratamiento t)
        {
            return new TratamientoRespuesta
            {
                Id = t.Id,
                IdPaciente = t.IdPaciente,
                Paciente = t.Paciente == null ? null : new PacienteRespuesta
                {
                    Id = t.Paciente.Id, Nombres = t.Paciente.Nombres, Apellidos = t.Paciente.Apellidos,
                    CI = t.Paciente.CI, FechaNacimiento = t.Paciente.FechaNacimiento, Telefono = t.Paciente.Telefono,
                    Email = t.Paciente.Email, Direccion = t.Paciente.Direccion, FechaRegistro = t.Paciente.FechaRegistro,
                    NombreCompleto = t.Paciente.NombreCompleto
                },
                IdOdontologo = t.IdOdontologo,
                Odontologo = t.Odontologo == null ? null : new OdontologoRespuesta
                {
                    Id = t.Odontologo.Id, Nombres = t.Odontologo.Nombres, Apellidos = t.Odontologo.Apellidos,
                    Matricula = t.Odontologo.Matricula, Telefono = t.Odontologo.Telefono, Email = t.Odontologo.Email,
                    IdEspecialidad = t.Odontologo.IdEspecialidad, Activo = t.Odontologo.Activo, NombreCompleto = t.Odontologo.NombreCompleto,
                    Especialidad = t.Odontologo.Especialidad == null ? null : new EspecialidadRespuesta { Id = t.Odontologo.Especialidad.Id, Nombre = t.Odontologo.Especialidad.Nombre, Descripcion = t.Odontologo.Especialidad.Descripcion },
                    Horarios = new List<HorarioRespuesta>()
                },
                IdCita = t.IdCita,
                Cita = t.Cita == null ? null : new CitaRespuesta
                {
                    Id = t.Cita.Id, IdPaciente = t.Cita.IdPaciente, IdOdontologo = t.Cita.IdOdontologo,
                    Fecha = t.Cita.Fecha, Hora = t.Cita.Hora, DuracionMinutos = t.Cita.DuracionMinutos,
                    Motivo = t.Cita.Motivo, Estado = t.Cita.Estado, MotivoCancelacion = t.Cita.MotivoCancelacion,
                    FechaRegistro = t.Cita.FechaRegistro, HoraFin = t.Cita.HoraFin, EstaVigente = t.Cita.EstaVigente
                },
                Tipo = t.Tipo,
                Fecha = t.Fecha,
                Costo = t.Costo,
                Observaciones = t.Observaciones
            };
        }
    }
}
