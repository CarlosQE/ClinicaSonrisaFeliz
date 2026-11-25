using ClinicaDental.Contrato.DTOs.Paciente;
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
    public class PacienteServicio : IPaciente
    {
        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public PacienteServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<PacienteRespuesta> ObtenerTodos()
        {
            return _context.pacientes
                .AsNoTracking()
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToList()
                .Select(Mapear);
        }

        public PacienteRespuesta? ObtenerPorId(Guid id)
        {
            var paciente = _context.pacientes
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
            return paciente == null ? null : Mapear(paciente);
        }

        public HistorialPacienteRespuesta? ObtenerHistorial(Guid id)
        {
            var paciente = _context.pacientes
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return null;
            }

            var citas = _context.citas
                .Include(c => c.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .AsNoTracking()
                .Where(c => c.IdPaciente == id)
                .OrderByDescending(c => c.Fecha).ThenByDescending(c => c.Hora)
                .ToList();

            var tratamientos = _context.tratamientos
                .Include(t => t.Odontologo)
                    .ThenInclude(o => o!.Especialidad)
                .Include(t => t.Cita)
                .AsNoTracking()
                .Where(t => t.IdPaciente == id)
                .OrderByDescending(t => t.Fecha)
                .ToList();

            return new HistorialPacienteRespuesta
            {
                Paciente = Mapear(paciente),
                Citas = citas.Select(c => new CitaRespuesta
                {
                    Id = c.Id,
                    IdPaciente = c.IdPaciente,
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
                }).ToList(),
                Tratamientos = tratamientos.Select(t => new TratamientoRespuesta
                {
                    Id = t.Id,
                    IdPaciente = t.IdPaciente,
                    IdOdontologo = t.IdOdontologo,
                    IdCita = t.IdCita,
                    Odontologo = t.Odontologo == null ? null : MapearOdontologo(t.Odontologo),
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
                }).ToList()
            };
        }

        public PacienteRespuesta Registrar(PacienteDto pacienteDto)
        {
            var paciente = new Paciente
            {
                Id = Guid.NewGuid(),
                Nombres = pacienteDto.Nombres.Trim(),
                Apellidos = pacienteDto.Apellidos.Trim(),
                CI = pacienteDto.CI.Trim(),
                FechaNacimiento = pacienteDto.FechaNacimiento!.Value,
                Telefono = pacienteDto.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(pacienteDto.Email) ? null : pacienteDto.Email.Trim(),
                Direccion = string.IsNullOrWhiteSpace(pacienteDto.Direccion) ? null : pacienteDto.Direccion.Trim(),
                FechaRegistro = Ahora
            };

            _context.pacientes.Add(paciente);
            _context.SaveChanges();

            return Mapear(paciente);
        }

        public PacienteRespuesta Actualizar(Guid id, PacienteDto pacienteDto)
        {
            var paciente = _context.pacientes.First(p => p.Id == id);

            paciente.Nombres = pacienteDto.Nombres.Trim();
            paciente.Apellidos = pacienteDto.Apellidos.Trim();
            paciente.CI = pacienteDto.CI.Trim();
            paciente.FechaNacimiento = pacienteDto.FechaNacimiento!.Value;
            paciente.Telefono = pacienteDto.Telefono.Trim();
            paciente.Email = string.IsNullOrWhiteSpace(pacienteDto.Email) ? null : pacienteDto.Email.Trim();
            paciente.Direccion = string.IsNullOrWhiteSpace(pacienteDto.Direccion) ? null : pacienteDto.Direccion.Trim();

            _context.SaveChanges();

            return Mapear(paciente);
        }

        public void Eliminar(Guid id)
        {
            var paciente = _context.pacientes.First(p => p.Id == id);

            _context.pacientes.Remove(paciente);
            _context.SaveChanges();
        }

        public bool DatosObligatoriosValidos(string nombres, string apellidos, string ci, string telefono, DateOnly fechaNacimiento)
        {
            return !string.IsNullOrWhiteSpace(nombres)
                && !string.IsNullOrWhiteSpace(apellidos)
                && !string.IsNullOrWhiteSpace(ci)
                && !string.IsNullOrWhiteSpace(telefono)
                && fechaNacimiento != default;
        }

        public bool NombreValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            var n = nombre.Trim();
            return n.Length >= 2 && n.Length <= 50
                && !TieneCaracteresDeControl(n)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\p{L}+([ '\-]\p{L}+)*$");
        }

        public bool CiValido(string ci)
        {
            if (string.IsNullOrWhiteSpace(ci)) return false;
            var n = ci.Trim();
            return !TieneCaracteresDeControl(n)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\d{5,10}(-[A-Za-z0-9]{1,2})?$");
        }

        public bool TelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;
            var n = telefono.Trim();
            return !TieneCaracteresDeControl(n)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\+?\d{7,15}$");
        }

        public bool EmailValido(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            var n = email.Trim();
            if (n.Length > 100 || TieneCaracteresDeControl(n)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(n, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool DireccionValida(string? direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion)) return true;
            var n = direccion.Trim();
            return n.Length <= 150 && !TieneCaracteresDeControl(n);
        }

        public bool FechaNacimientoValida(DateOnly fechaNacimiento)
        {
            var hoy = DateOnly.FromDateTime(Ahora);
            if (fechaNacimiento > hoy) return false;
            if (fechaNacimiento < new DateOnly(1900, 1, 1)) return false;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento > hoy.AddYears(-edad)) edad--;
            return edad <= 120;
        }

        private static bool TieneCaracteresDeControl(string valor)
        {
            return valor.Any(char.IsControl);
        }

        public bool ExistePaciente(Guid id)
        {
            return _context.pacientes.Any(p => p.Id == id);
        }

        public bool CiRepetido(string ci, Guid? excluirId = null)
        {
            var ciNormalizado = (ci ?? string.Empty).Trim();

            return _context.pacientes.Any(p =>
                p.CI == ciNormalizado &&
                (!excluirId.HasValue || p.Id != excluirId.Value));
        }

        public bool TieneCitas(Guid idPaciente)
        {
            return _context.citas.Any(c => c.IdPaciente == idPaciente);
        }

        public bool TieneTratamientos(Guid idPaciente)
        {
            return _context.tratamientos.Any(t => t.IdPaciente == idPaciente);
        }

        private static PacienteRespuesta Mapear(Paciente p)
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
