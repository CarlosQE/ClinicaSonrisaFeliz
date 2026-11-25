using ClinicaDental.Contrato.DTOs.Especialidad;
using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    public class EspecialidadServicio : IEspecialidad
    {
        private readonly BaseDatos _context;

        public EspecialidadServicio(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<EspecialidadRespuesta> ObtenerTodas()
        {
            return _context.especialidades
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .ToList()
                .Select(Mapear);
        }

        public EspecialidadRespuesta? ObtenerPorId(Guid id)
        {
            var e = _context.especialidades.AsNoTracking().FirstOrDefault(e => e.Id == id);
            return e == null ? null : Mapear(e);
        }

        public EspecialidadRespuesta Registrar(EspecialidadDto especialidadDto)
        {
            var especialidad = new Especialidad
            {
                Id = Guid.NewGuid(),
                Nombre = especialidadDto.Nombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(especialidadDto.Descripcion)
                    ? null
                    : especialidadDto.Descripcion.Trim()
            };

            _context.especialidades.Add(especialidad);
            _context.SaveChanges();

            return Mapear(especialidad);
        }

        public EspecialidadRespuesta Actualizar(Guid id, EspecialidadDto especialidadDto)
        {
            var especialidad = _context.especialidades.First(e => e.Id == id);

            especialidad.Nombre = especialidadDto.Nombre.Trim();
            especialidad.Descripcion = string.IsNullOrWhiteSpace(especialidadDto.Descripcion)
                ? null
                : especialidadDto.Descripcion.Trim();

            _context.SaveChanges();

            return Mapear(especialidad);
        }

        public void Eliminar(Guid id)
        {
            var especialidad = _context.especialidades.First(e => e.Id == id);
            _context.especialidades.Remove(especialidad);
            _context.SaveChanges();
        }

        public bool NombreValido(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length <= 80;
        }

        public bool NombreRepetido(string nombre, Guid? excluirId = null)
        {
            var nombreNormalizado = (nombre ?? string.Empty).Trim().ToLower();

            return _context.especialidades.Any(e =>
                e.Nombre.ToLower() == nombreNormalizado &&
                (!excluirId.HasValue || e.Id != excluirId.Value));
        }

        public bool TieneOdontologos(Guid idEspecialidad)
        {
            return _context.odontologos.Any(o => o.IdEspecialidad == idEspecialidad);
        }

        private static EspecialidadRespuesta Mapear(Especialidad e) => new()
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion
        };
    }
}
