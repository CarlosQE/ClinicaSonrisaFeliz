using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Especialidad
{
    /// <summary>Cuerpo de POST/PUT /api/Especialidad.</summary>
    public class EspecialidadDto
    {
        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres.")]
        [RegularExpression(@"^[\p{L} \-]+$", ErrorMessage = "El nombre solo puede contener letras, espacios y guion.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}
