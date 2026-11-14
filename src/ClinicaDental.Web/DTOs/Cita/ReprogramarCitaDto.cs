using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Cita
{
    public class ReprogramarCitaDto
    {
        [Required(ErrorMessage = "La nueva fecha y hora son requeridas")]
        public DateTime NuevaFechaHora { get; set; }

        public Guid? IdNuevoOdontologo { get; set; }
    }
}
