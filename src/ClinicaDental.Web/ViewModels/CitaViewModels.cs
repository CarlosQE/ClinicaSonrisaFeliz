using System.ComponentModel.DataAnnotations;
using ClinicaDental.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicaDental.Web.ViewModels;

/// <summary>Formulario para agendar una cita.</summary>
public class AgendarCitaViewModel
{
    [Required(ErrorMessage = "Seleccione un paciente.")]
    [Display(Name = "Paciente")]
    public int? PacienteId { get; set; }

    [Required(ErrorMessage = "Seleccione un odontólogo.")]
    [Display(Name = "Odontólogo")]
    public int? OdontologoId { get; set; }

    [Required(ErrorMessage = "Indique fecha y hora.")]
    [Display(Name = "Fecha y hora")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaHora { get; set; }

    [Range(15, 240, ErrorMessage = "La duración debe estar entre 15 y 240 minutos.")]
    [Display(Name = "Duración (min)")]
    public int DuracionMinutos { get; set; } = 30;

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(250)]
    public string Motivo { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Pacientes { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Odontologos { get; set; } = Enumerable.Empty<SelectListItem>();
}

/// <summary>Formulario para reprogramar una cita.</summary>
public class ReprogramarCitaViewModel
{
    public int Id { get; set; }

    public Cita? Cita { get; set; }

    [Required(ErrorMessage = "Indique la nueva fecha y hora.")]
    [Display(Name = "Nueva fecha y hora")]
    [DataType(DataType.DateTime)]
    public DateTime? NuevaFechaHora { get; set; }

    [Display(Name = "Odontólogo")]
    public int? OdontologoId { get; set; }

    public IEnumerable<SelectListItem> Odontologos { get; set; } = Enumerable.Empty<SelectListItem>();
}

/// <summary>Formulario para cancelar una cita.</summary>
public class CancelarCitaViewModel
{
    public int Id { get; set; }

    public Cita? Cita { get; set; }

    [Required(ErrorMessage = "Indique el motivo de la cancelación.")]
    [StringLength(250)]
    [Display(Name = "Motivo de cancelación")]
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>Pantalla de agenda diaria.</summary>
public class AgendaDiariaViewModel
{
    public DateOnly Fecha { get; set; }
    public int? OdontologoId { get; set; }
    public bool IncluirCanceladas { get; set; }
    public List<Cita> Citas { get; set; } = new();
    public IEnumerable<SelectListItem> Odontologos { get; set; } = Enumerable.Empty<SelectListItem>();
}

/// <summary>Listado general de citas con filtros.</summary>
public class ListadoCitasViewModel
{
    public DateOnly? Desde { get; set; }
    public DateOnly? Hasta { get; set; }
    public int? OdontologoId { get; set; }
    public EstadoCita? Estado { get; set; }
    public List<Cita> Citas { get; set; } = new();
    public IEnumerable<SelectListItem> Odontologos { get; set; } = Enumerable.Empty<SelectListItem>();
}
