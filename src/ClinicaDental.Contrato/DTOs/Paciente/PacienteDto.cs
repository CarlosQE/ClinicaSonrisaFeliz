using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Paciente
{
    /// <summary>Cuerpo de POST /api/Paciente y PUT /api/Paciente/{id}.</summary>
    public class PacienteDto
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

        [Required(ErrorMessage = "El CI es obligatorio.")]
        [RegularExpression(@"^\d{5,10}(-[A-Za-z0-9]{1,2})?$", ErrorMessage = "El CI debe tener 5 a 10 dígitos, con complemento opcional -1 o -1A.")]
        [Display(Name = "CI")]
        public string CI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "El teléfono debe tener 7 a 15 dígitos, con + opcional.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(150, ErrorMessage = "La dirección no puede tener más de 150 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }
}
