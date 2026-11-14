using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class Especialidad
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(80)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
