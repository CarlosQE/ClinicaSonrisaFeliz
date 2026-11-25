using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Paciente
{
    /// <summary>Cuerpo de POST /api/Paciente y PUT /api/Paciente/{id}.</summary>
    public class PacienteDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CI es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El CI no puede tener más de 20 caracteres.")]
        [Display(Name = "CI")]
        public string CI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "La dirección no puede tener más de 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }
}
