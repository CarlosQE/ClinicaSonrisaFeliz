using ClinicaDental.Contrato.DTOs.Pago;
using ClinicaDental.Contrato.Respuestas.Cita;
using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Contrato.Respuestas.Paciente;
using ClinicaDental.Contrato.Respuestas.Pago;
using ClinicaDental.Contrato.Respuestas.Tratamiento;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    public class PagoServicio : IPago
    {
        public const string PrefijoComprobante = "CMP-";

        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public PagoServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<PagoRespuesta> ObtenerTodos(Guid? idPaciente = null)
        {
            var query = _context.pagos
                .AsNoTracking()
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Paciente)
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Odontologo)
                        .ThenInclude(o => o!.Especialidad)
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Cita)
                .AsQueryable();

            if (idPaciente.HasValue)
            {
                query = query.Where(p => p.Tratamiento!.IdPaciente == idPaciente.Value);
            }

            return query.OrderByDescending(p => p.FechaPago).ToList().Select(Mapear);
        }

        public PagoRespuesta? ObtenerPorId(Guid id)
        {
            var pago = _context.pagos
                .AsNoTracking()
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Paciente)
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Odontologo)
                        .ThenInclude(o => o!.Especialidad)
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Cita)
                .FirstOrDefault(p => p.Id == id);
            return pago == null ? null : Mapear(pago);
        }

        public PagoRespuesta registrar(PagoDto pagoDto)
        {
            var pago = new Pago
            {
                Id = Guid.NewGuid(),
                IdTratamiento = pagoDto.IdTratamiento!.Value,
                Monto = pagoDto.Monto,
                FechaPago = Ahora,
                Metodo = pagoDto.Metodo,
                NumeroComprobante = GenerarNumeroComprobante(),
                Observacion = string.IsNullOrWhiteSpace(pagoDto.Observacion) ? null : pagoDto.Observacion.Trim()
            };

            _context.pagos.Add(pago);
            _context.SaveChanges();

            return ObtenerPorId(pago.Id)!;
        }

        public IEnumerable<DeudaRespuesta> ConsultarDeudas(Guid? idPaciente = null)
        {
            var queryTratamientos = _context.tratamientos
                .AsNoTracking()
                .Include(t => t.Paciente)
                .AsQueryable();

            if (idPaciente.HasValue)
            {
                queryTratamientos = queryTratamientos.Where(t => t.IdPaciente == idPaciente.Value);
            }

            var tratamientos = queryTratamientos.ToList();

            var pagadoPorTratamiento = _context.pagos
                .AsNoTracking()
                .Select(p => new { p.IdTratamiento, p.Monto })
                .ToList()
                .GroupBy(p => p.IdTratamiento)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Monto));

            return tratamientos
                .Select(t =>
                {
                    var pagado = pagadoPorTratamiento.GetValueOrDefault(t.Id);
                    return new { Tratamiento = t, Pagado = pagado, Saldo = t.Costo - pagado };
                })
                .Where(x => x.Saldo > 0)
                .OrderBy(x => x.Tratamiento.Paciente?.Apellidos)
                .ThenBy(x => x.Tratamiento.Paciente?.Nombres)
                .ThenBy(x => x.Tratamiento.Fecha)
                .Select(x => new DeudaRespuesta
                {
                    IdTratamiento = x.Tratamiento.Id,
                    IdPaciente = x.Tratamiento.IdPaciente,
                    PacienteNombre = x.Tratamiento.Paciente?.NombreCompleto ?? string.Empty,
                    PacienteCI = x.Tratamiento.Paciente?.CI ?? string.Empty,
                    Tipo = x.Tratamiento.Tipo.ToString(),
                    Fecha = x.Tratamiento.Fecha,
                    Costo = x.Tratamiento.Costo,
                    TotalPagado = x.Pagado,
                    Saldo = x.Saldo
                })
                .ToList();
        }

        public decimal ObtenerSaldoPendiente(Guid idTratamiento)
        {
            var costo = _context.tratamientos
                .AsNoTracking()
                .Where(t => t.Id == idTratamiento)
                .Select(t => t.Costo)
                .First();

            var pagado = _context.pagos
                .AsNoTracking()
                .Where(p => p.IdTratamiento == idTratamiento)
                .Select(p => p.Monto)
                .ToList()
                .Sum();

            return costo - pagado;
        }

        public ComprobanteRespuesta? ObtenerComprobante(Guid idPago)
        {
            var pago = _context.pagos
                .AsNoTracking()
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Paciente)
                .FirstOrDefault(p => p.Id == idPago);

            if (pago == null || pago.Tratamiento == null)
            {
                return null;
            }

            var tratamiento = pago.Tratamiento;

            var pagosDelTratamiento = _context.pagos
                .AsNoTracking()
                .Where(p => p.IdTratamiento == pago.IdTratamiento)
                .ToList();

            var pagadoHastaEstePago = pagosDelTratamiento
                .Where(p => p.FechaPago < pago.FechaPago
                         || (p.FechaPago == pago.FechaPago
                             && string.CompareOrdinal(p.NumeroComprobante, pago.NumeroComprobante) <= 0))
                .Sum(p => p.Monto);

            return new ComprobanteRespuesta
            {
                IdPago = pago.Id,
                NumeroComprobante = pago.NumeroComprobante,
                FechaPago = pago.FechaPago,
                PacienteNombre = tratamiento.Paciente?.NombreCompleto ?? string.Empty,
                PacienteCI = tratamiento.Paciente?.CI ?? string.Empty,
                Tratamiento = tratamiento.Tipo.ToString(),
                FechaTratamiento = tratamiento.Fecha,
                CostoTratamiento = tratamiento.Costo,
                Monto = pago.Monto,
                Metodo = pago.Metodo.ToString(),
                Observacion = pago.Observacion,
                SaldoRestante = tratamiento.Costo - pagadoHastaEstePago
            };
        }

        public bool ExistePaciente(Guid idPaciente)
        {
            return _context.pacientes.Any(p => p.Id == idPaciente);
        }

        public bool ExisteTratamiento(Guid idTratamiento)
        {
            return _context.tratamientos.Any(t => t.Id == idTratamiento);
        }

        public bool MontoValido(decimal monto)
        {
            return monto > 0 && monto <= 1000000m && decimal.Round(monto, 2) == monto;
        }

        public bool ObservacionValida(string? observacion)
        {
            if (string.IsNullOrWhiteSpace(observacion)) return true;
            var n = observacion.Trim();
            return n.Length <= 250 && !n.Any(char.IsControl);
        }

        public bool MetodoValido(MetodoPago metodo)
        {
            return Enum.IsDefined(metodo);
        }

        public bool MontoNoExcedeSaldo(Guid idTratamiento, decimal monto)
        {
            return monto <= ObtenerSaldoPendiente(idTratamiento);
        }

        private string GenerarNumeroComprobante()
        {
            var ultimo = _context.pagos
                .AsNoTracking()
                .Where(p => p.NumeroComprobante.StartsWith(PrefijoComprobante))
                .OrderByDescending(p => p.NumeroComprobante)
                .Select(p => p.NumeroComprobante)
                .FirstOrDefault();

            var numero = 0;
            if (ultimo != null && int.TryParse(ultimo.Substring(PrefijoComprobante.Length), out var actual))
            {
                numero = actual;
            }

            return $"{PrefijoComprobante}{numero + 1:D6}";
        }

        private static PagoRespuesta Mapear(Pago p)
        {
            return new PagoRespuesta
            {
                Id = p.Id,
                IdTratamiento = p.IdTratamiento,
                Tratamiento = p.Tratamiento == null ? null : new TratamientoRespuesta
                {
                    Id = p.Tratamiento.Id,
                    IdPaciente = p.Tratamiento.IdPaciente,
                    Paciente = p.Tratamiento.Paciente == null ? null : new PacienteRespuesta
                    {
                        Id = p.Tratamiento.Paciente.Id, Nombres = p.Tratamiento.Paciente.Nombres, Apellidos = p.Tratamiento.Paciente.Apellidos,
                        CI = p.Tratamiento.Paciente.CI, FechaNacimiento = p.Tratamiento.Paciente.FechaNacimiento, Telefono = p.Tratamiento.Paciente.Telefono,
                        Email = p.Tratamiento.Paciente.Email, Direccion = p.Tratamiento.Paciente.Direccion, FechaRegistro = p.Tratamiento.Paciente.FechaRegistro,
                        NombreCompleto = p.Tratamiento.Paciente.NombreCompleto
                    },
                    IdOdontologo = p.Tratamiento.IdOdontologo,
                    Odontologo = p.Tratamiento.Odontologo == null ? null : new OdontologoRespuesta
                    {
                        Id = p.Tratamiento.Odontologo.Id, Nombres = p.Tratamiento.Odontologo.Nombres, Apellidos = p.Tratamiento.Odontologo.Apellidos,
                        Matricula = p.Tratamiento.Odontologo.Matricula, Telefono = p.Tratamiento.Odontologo.Telefono, Email = p.Tratamiento.Odontologo.Email,
                        IdEspecialidad = p.Tratamiento.Odontologo.IdEspecialidad, Activo = p.Tratamiento.Odontologo.Activo, NombreCompleto = p.Tratamiento.Odontologo.NombreCompleto,
                        Especialidad = p.Tratamiento.Odontologo.Especialidad == null ? null : new EspecialidadRespuesta { Id = p.Tratamiento.Odontologo.Especialidad.Id, Nombre = p.Tratamiento.Odontologo.Especialidad.Nombre, Descripcion = p.Tratamiento.Odontologo.Especialidad.Descripcion },
                        Horarios = new List<HorarioRespuesta>()
                    },
                    IdCita = p.Tratamiento.IdCita,
                    Cita = p.Tratamiento.Cita == null ? null : new CitaRespuesta
                    {
                        Id = p.Tratamiento.Cita.Id, IdPaciente = p.Tratamiento.Cita.IdPaciente, IdOdontologo = p.Tratamiento.Cita.IdOdontologo,
                        Fecha = p.Tratamiento.Cita.Fecha, Hora = p.Tratamiento.Cita.Hora, DuracionMinutos = p.Tratamiento.Cita.DuracionMinutos,
                        Motivo = p.Tratamiento.Cita.Motivo, Estado = p.Tratamiento.Cita.Estado, MotivoCancelacion = p.Tratamiento.Cita.MotivoCancelacion,
                        FechaRegistro = p.Tratamiento.Cita.FechaRegistro, HoraFin = p.Tratamiento.Cita.HoraFin, EstaVigente = p.Tratamiento.Cita.EstaVigente
                    },
                    Tipo = p.Tratamiento.Tipo,
                    Fecha = p.Tratamiento.Fecha,
                    Costo = p.Tratamiento.Costo,
                    Observaciones = p.Tratamiento.Observaciones
                },
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                Metodo = p.Metodo,
                NumeroComprobante = p.NumeroComprobante,
                Observacion = p.Observacion
            };
        }
    }
}
