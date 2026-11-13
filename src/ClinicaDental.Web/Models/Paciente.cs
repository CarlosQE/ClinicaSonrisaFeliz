using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Paciente de la clínica. Dueño funcional: módulo 1 (Pacientes).
/// CONTRATO: las propiedades existentes no se renombran ni se eliminan.
/// </summary>
public class Paciente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>Carnet de identidad. Único en el sistema.</summary>
    [Required(ErrorMessage = "El CI es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "CI")]
    public string CI { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email no válido.")]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; }

    public string NombreCompleto => $"{Nombres} {Apellidos}";

    public List<Cita> Citas { get; set; } = new();
    public List<Tratamiento> Tratamientos { get; set; } = new();
}
