using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Cita odontológica. Dueño funcional: módulo 3 (Citas - AndresFundamentalistas).
/// CONTRATO: las propiedades existentes no se renombran ni se eliminan.
/// </summary>
public class Cita
{
    public int Id { get; set; }

    [Display(Name = "Paciente")]
    public int PacienteId { get; set; }
    public Paciente? Paciente { get; set; }

    [Display(Name = "Odontólogo")]
    public int OdontologoId { get; set; }
    public Odontologo? Odontologo { get; set; }

    [Display(Name = "Fecha y hora")]
    public DateTime FechaHora { get; set; }

    [Range(15, 240, ErrorMessage = "La duración debe estar entre 15 y 240 minutos.")]
    [Display(Name = "Duración (min)")]
    public int DuracionMinutos { get; set; } = 30;

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(250)]
    public string Motivo { get; set; } = string.Empty;

    public EstadoCita Estado { get; set; } = EstadoCita.Programada;

    [StringLength(250)]
    [Display(Name = "Motivo de cancelación")]
    public string? MotivoCancelacion { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; }

    public DateTime FechaHoraFin => FechaHora.AddMinutes(DuracionMinutos);

    /// <summary>Una cita "ocupa" el horario mientras no esté cancelada.</summary>
    public bool OcupaHorario => Estado != EstadoCita.Cancelada;

    /// <summary>Solo se puede reprogramar, cancelar o atender una cita Programada o Reprogramada.</summary>
    public bool EstaVigente => Estado is EstadoCita.Programada or EstadoCita.Reprogramada;

    public List<Tratamiento> Tratamientos { get; set; } = new();
}
