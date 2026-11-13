using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Tratamiento realizado a un paciente. Dueño funcional: módulo 4 (Tratamientos).
/// CONTRATO: las propiedades existentes no se renombran ni se eliminan.
/// </summary>
public class Tratamiento
{
    public int Id { get; set; }

    [Display(Name = "Paciente")]
    public int PacienteId { get; set; }
    public Paciente? Paciente { get; set; }

    [Display(Name = "Odontólogo")]
    public int OdontologoId { get; set; }
    public Odontologo? Odontologo { get; set; }

    /// <summary>Cita en la que se realizó (opcional: una ortodoncia puede abarcar varias).</summary>
    [Display(Name = "Cita")]
    public int? CitaId { get; set; }
    public Cita? Cita { get; set; }

    [Display(Name = "Tipo de tratamiento")]
    public TipoTratamiento Tipo { get; set; }

    [DataType(DataType.Date)]
    public DateOnly Fecha { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "El costo debe ser mayor o igual a 0.")]
    public decimal Costo { get; set; }

    [StringLength(1000)]
    public string? Observaciones { get; set; }

    public List<Pago> Pagos { get; set; } = new();
}
