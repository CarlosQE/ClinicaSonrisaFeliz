using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Especialidad
{
    /// <summary>Cuerpo de POST/PUT /api/Especialidad.</summary>
    public class EspecialidadDto
    {
        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [MaxLength(80, ErrorMessage = "El nombre no puede tener más de 80 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}
