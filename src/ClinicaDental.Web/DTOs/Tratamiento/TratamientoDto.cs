using System.ComponentModel.DataAnnotations;
using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.DTOs.Tratamiento
{
    public class TratamientoDto
    {
        [Required(ErrorMessage = "El paciente es obligatorio.")]
        public Guid IdPaciente { get; set; }

        [Required(ErrorMessage = "El odontólogo es obligatorio.")]
        public Guid IdOdontologo { get; set; }

        public Guid? IdCita { get; set; }

        // 0 = LimpiezaDental, 1 = Ortodoncia, 2 = Endodoncia, 3 = Extraccion
        public TipoTratamiento Tipo { get; set; }

        public DateOnly? Fecha { get; set; }

        public decimal Costo { get; set; }

        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
