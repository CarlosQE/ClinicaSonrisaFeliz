using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Tratamiento
{
    public class ActualizarObservacionesDto
    {
        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres.")]
        public string Observaciones { get; set; } = string.Empty;
    }
}
