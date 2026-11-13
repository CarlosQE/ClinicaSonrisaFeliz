using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Franja de trabajo semanal de un odontólogo. Dueño funcional: módulo 2.
/// El módulo 3 (Citas) la USA para validar que una cita cae dentro del horario.
/// Un odontólogo puede tener varias franjas el mismo día (ej. mañana y tarde).
/// </summary>
public class HorarioOdontologo
{
    public int Id { get; set; }

    public int OdontologoId { get; set; }
    public Odontologo? Odontologo { get; set; }

    [Display(Name = "Día")]
    public DayOfWeek DiaSemana { get; set; }

    [Display(Name = "Hora inicio")]
    public TimeOnly HoraInicio { get; set; }

    [Display(Name = "Hora fin")]
    public TimeOnly HoraFin { get; set; }
}
