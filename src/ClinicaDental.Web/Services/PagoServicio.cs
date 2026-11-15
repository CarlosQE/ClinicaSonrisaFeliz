using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Pago;
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

        public IEnumerable<Pago> ObtenerTodos(Guid? idPaciente = null)
        {
            var query = _context.pagos
                .AsNoTracking()
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Paciente)
                .AsQueryable();

            if (idPaciente.HasValue)
            {
                query = query.Where(p => p.Tratamiento!.IdPaciente == idPaciente.Value);
            }

            return query.OrderByDescending(p => p.FechaPago).ToList();
        }

        public Pago? ObtenerPorId(Guid id)
        {
            return _context.pagos
                .AsNoTracking()
                .Include(p => p.Tratamiento)
                    .ThenInclude(t => t!.Paciente)
                .FirstOrDefault(p => p.Id == id);
        }

        public Pago registrar(PagoDto pagoDto)
        {
            var pago = new Pago
            {
                Id = Guid.NewGuid(),
                IdTratamiento = pagoDto.IdTratamiento,
                Monto = pagoDto.Monto,
                FechaPago = Ahora,
                Metodo = pagoDto.Metodo,
                NumeroComprobante = GenerarNumeroComprobante(),
                Observacion = string.IsNullOrWhiteSpace(pagoDto.Observacion) ? null : pagoDto.Observacion.Trim()
            };

            _context.pagos.Add(pago);
            _context.SaveChanges();

            return pago;
        }

        public IEnumerable<DeudaDto> ConsultarDeudas(Guid? idPaciente = null)
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
                .Select(x => new DeudaDto
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

        public ComprobanteDto? ObtenerComprobante(Guid idPago)
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

            // Lo pagado hasta este pago inclusive (el orden lo da la fecha y, si empatan, el comprobante).
            var pagadoHastaEstePago = pagosDelTratamiento
                .Where(p => p.FechaPago < pago.FechaPago
                         || (p.FechaPago == pago.FechaPago
                             && string.CompareOrdinal(p.NumeroComprobante, pago.NumeroComprobante) <= 0))
                .Sum(p => p.Monto);

            return new ComprobanteDto
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
            return monto > 0 && decimal.Round(monto, 2) == monto;
        }

        public bool MetodoValido(MetodoPago metodo)
        {
            return Enum.IsDefined(metodo);
        }

        public bool MontoNoExcedeSaldo(Guid idTratamiento, decimal monto)
        {
            return monto <= ObtenerSaldoPendiente(idTratamiento);
        }

        // CMP-000001, CMP-000002... correlativo al mayor comprobante existente.
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
    }
}