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
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            var n = nombre.Trim();
            return n.Length >= 3 && n.Length <= 60
                && !n.Any(char.IsControl)
                && System.Text.RegularExpressions.Regex.IsMatch(n, @"^[\p{L} \-]+$");
        }

        public bool DescripcionValida(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion)) return true;
            var n = descripcion.Trim();
            return n.Length <= 200 && !n.Any(char.IsControl);
        }

        public bool NombreRepetido(string nombre, Guid? excluirId = null)
        {
            var buscado = Normalizar(nombre);

            // Son muy pocas especialidades: se traen los nombres y se comparan sin tildes ni mayúsculas.
            return _context.especialidades
                .AsNoTracking()
                .Where(e => !excluirId.HasValue || e.Id != excluirId.Value)
                .Select(e => e.Nombre)
                .ToList()
                .Any(n => Normalizar(n) == buscado);
        }

        public bool TieneOdontologos(Guid idEspecialidad)
        {
            return _context.odontologos.Any(o => o.IdEspecialidad == idEspecialidad);
        }

        // Compara sin distinguir mayúsculas, tildes ni espacios sobrantes.
        private static string Normalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            var descompuesto = texto.Trim().Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in descompuesto)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return System.Text.RegularExpressions.Regex
                .Replace(sb.ToString(), @"\s+", " ")
                .ToLowerInvariant();
        }

        private static EspecialidadRespuesta Mapear(Especialidad e) => new()
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion
        };
    }
}