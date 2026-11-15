using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Especialidad
{
    public class EspecialidadDto
    {
        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [MaxLength(80, ErrorMessage = "El nombre no puede tener más de 80 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
