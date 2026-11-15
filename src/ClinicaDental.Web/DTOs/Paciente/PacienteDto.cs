using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Paciente
{
    public class PacienteDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CI es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El CI no puede tener más de 20 caracteres.")]
        public string CI { get; set; } = string.Empty;

        public DateOnly FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres.")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "La dirección no puede tener más de 250 caracteres.")]
        public string? Direccion { get; set; }
    }
}
