using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Tratamiento;
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

        public IEnumerable<Tratamiento> ObtenerTodos(Guid? idPaciente = null, TipoTratamiento? tipo = null, Guid? idOdontologo = null)
        {
            var query = _context.tratamientos
                .Include(t => t.Paciente)
                .Include(t => t.Odontologo)
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
                .ToList();
        }

        public Tratamiento? ObtenerPorId(Guid id)
        {
            return _context.tratamientos
                .Include(t => t.Paciente)
                .Include(t => t.Odontologo)
                .Include(t => t.Cita)
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);
        }

        public Tratamiento Registrar(TratamientoDto tratamientoDto)
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
                IdPaciente = tratamientoDto.IdPaciente,
                IdOdontologo = tratamientoDto.IdOdontologo,
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

            return tratamiento;
        }

        public Tratamiento ActualizarObservaciones(Guid id, string observaciones)
        {
            var tratamiento = _context.tratamientos.First(t => t.Id == id);

            tratamiento.Observaciones = observaciones.Trim();
            _context.SaveChanges();

            return tratamiento;
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
            return costo > 0 && decimal.Round(costo, 2) == costo;
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
    }
}
