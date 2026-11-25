using ClinicaDental.Contrato.DTOs.Odontologo;
using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;
using ClinicaDental.Contrato.Respuestas.Odontologo;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    public class OdontologoServicio : IOdontologo
    {
        private readonly BaseDatos _context;
        private readonly TimeProvider _reloj;

        public OdontologoServicio(BaseDatos context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        private DateTime Ahora => _reloj.GetLocalNow().DateTime;

        public IEnumerable<OdontologoRespuesta> ObtenerTodos(bool? activo = null, Guid? idEspecialidad = null)
        {
            var query = _context.odontologos
                .Include(o => o.Especialidad)
                .AsNoTracking()
                .AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(o => o.Activo == activo.Value);
            }

            if (idEspecialidad.HasValue)
            {
                query = query.Where(o => o.IdEspecialidad == idEspecialidad.Value);
            }

            var odontologos = query
                .OrderBy(o => o.Apellidos)
                .ThenBy(o => o.Nombres)
                .ToList();

            var ids = odontologos.Select(o => o.Id).ToList();
            var horarios = _context.horariosOdontologo
                .AsNoTracking()
                .Where(h => ids.Contains(h.IdOdontologo))
                .ToList()
                .GroupBy(h => h.IdOdontologo)
                .ToDictionary(g => g.Key, g => g.Select(MapearHorario).ToList());

            return odontologos.Select(o => Mapear(o, horarios.GetValueOrDefault(o.Id) ?? new List<HorarioRespuesta>()));
        }

        public OdontologoRespuesta? ObtenerPorId(Guid id)
        {
            var o = _context.odontologos
                .Include(o => o.Especialidad)
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == id);
            if (o == null) return null;
            var horarios = _context.horariosOdontologo.AsNoTracking().Where(h => h.IdOdontologo == id).ToList().Select(MapearHorario).ToList();
            return Mapear(o, horarios);
        }

        public OdontologoRespuesta Registrar(OdontologoDto odontologoDto)
        {
            var odontologo = new Odontologo
            {
                Id = Guid.NewGuid(),
                Nombres = odontologoDto.Nombres.Trim(),
                Apellidos = odontologoDto.Apellidos.Trim(),
                Matricula = odontologoDto.Matricula.Trim(),
                Telefono = odontologoDto.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(odontologoDto.Email) ? null : odontologoDto.Email.Trim(),
                Activo = true,
                IdEspecialidad = odontologoDto.IdEspecialidad!.Value
            };

            _context.odontologos.Add(odontologo);
            _context.SaveChanges();

            return ObtenerPorId(odontologo.Id)!;
        }

        public OdontologoRespuesta Actualizar(Guid id, OdontologoDto odontologoDto)
        {
            var odontologo = _context.odontologos.First(o => o.Id == id);

            odontologo.Nombres = odontologoDto.Nombres.Trim();
            odontologo.Apellidos = odontologoDto.Apellidos.Trim();
            odontologo.Matricula = odontologoDto.Matricula.Trim();
            odontologo.Telefono = odontologoDto.Telefono.Trim();
            odontologo.Email = string.IsNullOrWhiteSpace(odontologoDto.Email) ? null : odontologoDto.Email.Trim();
            odontologo.IdEspecialidad = odontologoDto.IdEspecialidad!.Value;

            _context.SaveChanges();

            return ObtenerPorId(id)!;
        }

        public OdontologoRespuesta CambiarEstado(Guid id, bool activo)
        {
            var odontologo = _context.odontologos.First(o => o.Id == id);
            odontologo.Activo = activo;
            _context.SaveChanges();

            return ObtenerPorId(id)!;
        }

        public bool DatosObligatoriosValidos(string nombres, string apellidos, string matricula, string telefono)
        {
            return !string.IsNullOrWhiteSpace(nombres)
                && !string.IsNullOrWhiteSpace(apellidos)
                && !string.IsNullOrWhiteSpace(matricula)
                && !string.IsNullOrWhiteSpace(telefono);
        }

        public bool NombreValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            var n = nombre.Trim();
            return n.Length >= 2 && n.Length <= 50
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\p{L}+([ '\-]\p{L}+)*$");
        }

        public bool MatriculaValida(string matricula)
        {
            if (string.IsNullOrWhiteSpace(matricula)) return false;
            var n = matricula.Trim();
            return n.Length >= 3 && n.Length <= 20
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^[A-Za-z0-9]{3,20}$");
        }

        public bool TelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;
            var n = telefono.Trim();
            return !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^\+?\d{7,15}$");
        }

        public bool EmailValido(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            var n = email.Trim();
            if (n.Length > 100 || n.Any(char.IsControl)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(n, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool ExisteOdontologo(Guid id)
        {
            return _context.odontologos.Any(o => o.Id == id);
        }

        public bool MatriculaRepetida(string matricula, Guid? excluirId = null)
        {
            var matriculaNormalizada = (matricula ?? string.Empty).Trim();

            return _context.odontologos.Any(o =>
                o.Matricula == matriculaNormalizada &&
                (!excluirId.HasValue || o.Id != excluirId.Value));
        }

        public bool ExisteEspecialidad(Guid idEspecialidad)
        {
            return _context.especialidades.Any(e => e.Id == idEspecialidad);
        }

        public bool TieneCitasFuturas(Guid idOdontologo)
        {
            var ahora = Ahora;
            var hoy = DateOnly.FromDateTime(ahora);
            var horaActual = TimeOnly.FromDateTime(ahora);

            return _context.citas.Any(c =>
                c.IdOdontologo == idOdontologo &&
                c.Estado != EstadoCita.Cancelada &&
                (c.Fecha > hoy || (c.Fecha == hoy && c.Hora > horaActual)));
        }

        private static OdontologoRespuesta Mapear(Odontologo o, List<HorarioRespuesta> horarios) => new()
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
            Horarios = horarios
        };

        private static HorarioRespuesta MapearHorario(HorarioOdontologo h) => new()
        {
            Id = h.Id,
            IdOdontologo = h.IdOdontologo,
            DiaSemana = h.DiaSemana,
            HoraInicio = h.HoraInicio,
            HoraFin = h.HoraFin
        };
    }
}
