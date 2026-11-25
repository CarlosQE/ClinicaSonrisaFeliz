using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Odontologo
{
    /// <summary>Cuerpo de POST /api/Odontologo y PUT /api/Odontologo/{id}.</summary>
    public class OdontologoDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [MaxLength(30, ErrorMessage = "La matrícula no puede tener más de 30 caracteres.")]
        [Display(Name = "Matrícula")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [Display(Name = "Especialidad")]
        public Guid? IdEspecialidad { get; set; }
    }
}
