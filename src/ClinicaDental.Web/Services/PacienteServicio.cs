using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Paciente;
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

        public IEnumerable<Paciente> ObtenerTodos()
        {
            return _context.pacientes
                .AsNoTracking()
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToList();
        }

        public Paciente? ObtenerPorId(Guid id)
        {
            return _context.pacientes
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public HistorialPacienteDto? ObtenerHistorial(Guid id)
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
                .AsNoTracking()
                .Where(c => c.IdPaciente == id)
                .OrderByDescending(c => c.FechaHora)
                .ToList();

            var tratamientos = _context.tratamientos
                .Include(t => t.Odontologo)
                .AsNoTracking()
                .Where(t => t.IdPaciente == id)
                .OrderByDescending(t => t.Fecha)
                .ToList();

            return new HistorialPacienteDto
            {
                Paciente = paciente,
                Citas = citas,
                Tratamientos = tratamientos
            };
        }

        public Paciente Registrar(PacienteDto pacienteDto)
        {
            var paciente = new Paciente
            {
                Id = Guid.NewGuid(),
                Nombres = pacienteDto.Nombres.Trim(),
                Apellidos = pacienteDto.Apellidos.Trim(),
                CI = pacienteDto.CI.Trim(),
                FechaNacimiento = pacienteDto.FechaNacimiento,
                Telefono = pacienteDto.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(pacienteDto.Email) ? null : pacienteDto.Email.Trim(),
                Direccion = string.IsNullOrWhiteSpace(pacienteDto.Direccion) ? null : pacienteDto.Direccion.Trim(),
                FechaRegistro = Ahora
            };

            _context.pacientes.Add(paciente);
            _context.SaveChanges();

            return paciente;
        }

        public Paciente Actualizar(Guid id, ActualizarPacienteDto pacienteDto)
        {
            var paciente = _context.pacientes.First(p => p.Id == id);

            paciente.Nombres = pacienteDto.Nombres.Trim();
            paciente.Apellidos = pacienteDto.Apellidos.Trim();
            paciente.CI = pacienteDto.CI.Trim();
            paciente.FechaNacimiento = pacienteDto.FechaNacimiento;
            paciente.Telefono = pacienteDto.Telefono.Trim();
            paciente.Email = string.IsNullOrWhiteSpace(pacienteDto.Email) ? null : pacienteDto.Email.Trim();
            paciente.Direccion = string.IsNullOrWhiteSpace(pacienteDto.Direccion) ? null : pacienteDto.Direccion.Trim();

            _context.SaveChanges();

            return paciente;
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

        public bool FechaNacimientoValida(DateOnly fechaNacimiento)
        {
            return fechaNacimiento <= DateOnly.FromDateTime(Ahora);
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
    }
}
