using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaDental.Web.Models
{
    [Index(nameof(Matricula), IsUnique = true)]
    public class Odontologo
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;
        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [MaxLength(30)]
        public string Matricula { get; set; } = string.Empty;
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Email no válido.")]
        [MaxLength(150)]
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;
        public Guid IdEspecialidad { get; set; }
        [ForeignKey(nameof(IdEspecialidad))]
        public virtual Especialidad? Especialidad { get; set; }

        public string NombreCompleto => $"Dr(a). {Nombres} {Apellidos}";
    }
}
