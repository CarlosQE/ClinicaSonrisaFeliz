using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>Especialidad odontológica. Dueño funcional: módulo 2 (Odontólogos).</summary>
public class Especialidad
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public List<Odontologo> Odontologos { get; set; } = new();
}
