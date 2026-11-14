using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models
{
    [Index(nameof(CI), IsUnique = true)]
    public class Paciente
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;
        [Required(ErrorMessage = "El CI es obligatorio.")]
        [MaxLength(20)]
        public string CI { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Email no válido.")]
        [MaxLength(150)]
        public string? Email { get; set; }
        [MaxLength(250)]
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}
