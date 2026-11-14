using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Odontologo
{
    public class ActualizarOdontologoDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [MaxLength(30, ErrorMessage = "La matrícula no puede tener más de 30 caracteres.")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        public Guid IdEspecialidad { get; set; }
    }
}
