using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Odontologo;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    /// <summary>
    /// Parte A del módulo 2: registro, actualización, consulta y estado de odontólogos.
    /// </summary>
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

        public IEnumerable<Odontologo> ObtenerTodos(bool? activo = null, Guid? idEspecialidad = null)
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

            return query
                .OrderBy(o => o.Apellidos)
                .ThenBy(o => o.Nombres)
                .ToList();
        }

        public Odontologo? ObtenerPorId(Guid id)
        {
            return _context.odontologos
                .Include(o => o.Especialidad)
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == id);
        }

        public Odontologo Registrar(OdontologoDto odontologoDto)
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
                IdEspecialidad = odontologoDto.IdEspecialidad
            };

            _context.odontologos.Add(odontologo);
            _context.SaveChanges();

            return odontologo;
        }

        public Odontologo Actualizar(Guid id, ActualizarOdontologoDto odontologoDto)
        {
            var odontologo = _context.odontologos.First(o => o.Id == id);

            odontologo.Nombres = odontologoDto.Nombres.Trim();
            odontologo.Apellidos = odontologoDto.Apellidos.Trim();
            odontologo.Matricula = odontologoDto.Matricula.Trim();
            odontologo.Telefono = odontologoDto.Telefono.Trim();
            odontologo.Email = string.IsNullOrWhiteSpace(odontologoDto.Email) ? null : odontologoDto.Email.Trim();
            odontologo.IdEspecialidad = odontologoDto.IdEspecialidad;

            _context.SaveChanges();

            return odontologo;
        }

        public Odontologo CambiarEstado(Guid id, bool activo)
        {
            var odontologo = _context.odontologos.First(o => o.Id == id);
            odontologo.Activo = activo;
            _context.SaveChanges();

            return odontologo;
        }

        public bool DatosObligatoriosValidos(string nombres, string apellidos, string matricula, string telefono)
        {
            return !string.IsNullOrWhiteSpace(nombres)
                && !string.IsNullOrWhiteSpace(apellidos)
                && !string.IsNullOrWhiteSpace(matricula)
                && !string.IsNullOrWhiteSpace(telefono);
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

            return _context.citas.Any(c =>
                c.IdOdontologo == idOdontologo &&
                c.Estado != EstadoCita.Cancelada &&
                c.FechaHora > ahora);
        }
    }
}
