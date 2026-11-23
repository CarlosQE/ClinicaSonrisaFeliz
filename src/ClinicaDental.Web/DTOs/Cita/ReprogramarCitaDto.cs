using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Cita
{
    public class ReprogramarCitaDto
    {
        [Required(ErrorMessage = "La nueva fecha es requerida")]
        public DateOnly? NuevaFecha { get; set; }

        [Required(ErrorMessage = "La nueva hora es requerida")]
        public TimeOnly? NuevaHora { get; set; }

        public Guid? IdNuevoOdontologo { get; set; }
    }
}
