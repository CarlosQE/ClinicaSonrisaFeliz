using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Cita
{
    /// <summary>Cuerpo de PUT /api/Cita/reprogramar/{id}.</summary>
    public class ReprogramarCitaDto
    {
        [Required(ErrorMessage = "La nueva fecha es requerida")]
        [Display(Name = "Nueva fecha")]
        public DateOnly? NuevaFecha { get; set; }

        [Required(ErrorMessage = "La nueva hora es requerida")]
        [Display(Name = "Nueva hora")]
        public TimeOnly? NuevaHora { get; set; }

        [Display(Name = "Nuevo odontólogo (opcional)")]
        public Guid? IdNuevoOdontologo { get; set; }
    }
}
