using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Odontólogo de la clínica. Dueño funcional: módulo 2 (Odontólogos).
/// CONTRATO: las propiedades existentes no se renombran ni se eliminan.
/// </summary>
public class Odontologo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>Matrícula profesional. Única en el sistema.</summary>
    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    [StringLength(30)]
    [Display(Name = "Matrícula")]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email no válido.")]
    [StringLength(150)]
    public string? Email { get; set; }

    /// <summary>Un odontólogo inactivo NO puede recibir citas nuevas.</summary>
    public bool Activo { get; set; } = true;

    [Display(Name = "Especialidad")]
    public int EspecialidadId { get; set; }
    public Especialidad? Especialidad { get; set; }

    public List<HorarioOdontologo> Horarios { get; set; } = new();
    public List<Cita> Citas { get; set; } = new();

    public string NombreCompleto => $"Dr(a). {Nombres} {Apellidos}";
}
