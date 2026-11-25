using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Odontologo
{
    /// <summary>Cuerpo de POST /api/Odontologo y PUT /api/Odontologo/{id}.</summary>
    public class OdontologoDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^\p{L}+([ '\-]\p{L}+)*$", ErrorMessage = "Los nombres solo pueden contener letras, espacios, apóstrofe o guion.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^\p{L}+([ '\-]\p{L}+)*$", ErrorMessage = "Los apellidos solo pueden contener letras, espacios, apóstrofe o guion.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [RegularExpression(@"^[A-Za-z0-9]{3,20}$", ErrorMessage = "La matrícula debe ser alfanumérica, de 3 a 20 caracteres, sin símbolos.")]
        [Display(Name = "Matrícula")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "El teléfono debe tener 7 a 15 dígitos, con + opcional.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [Display(Name = "Especialidad")]
        public Guid? IdEspecialidad { get; set; }
    }
}
