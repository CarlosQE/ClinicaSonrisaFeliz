using ClinicaDental.Web.Data;
using ClinicaDental.Web.DTOs.Especialidad;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services
{
    /// <summary>
    /// Parte B del módulo 2: CRUD de especialidades.
    /// </summary>
    public class EspecialidadServicio : IEspecialidad
    {
        private readonly BaseDatos _context;

        public EspecialidadServicio(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Especialidad> ObtenerTodas()
        {
            return _context.especialidades
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .ToList();
        }

        public Especialidad? ObtenerPorId(Guid id)
        {
            return _context.especialidades
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
        }

        public Especialidad Registrar(EspecialidadDto especialidadDto)
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

            return especialidad;
        }

        public Especialidad Actualizar(Guid id, EspecialidadDto especialidadDto)
        {
            var especialidad = _context.especialidades.First(e => e.Id == id);

            especialidad.Nombre = especialidadDto.Nombre.Trim();
            especialidad.Descripcion = string.IsNullOrWhiteSpace(especialidadDto.Descripcion)
                ? null
                : especialidadDto.Descripcion.Trim();

            _context.SaveChanges();

            return especialidad;
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
    }
}
