using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.Contrato.DTOs.Tratamiento
{
    /// <summary>Cuerpo de POST /api/Tratamiento.</summary>
    public class TratamientoDto
    {
        [Required(ErrorMessage = "El paciente es obligatorio.")]
        [Display(Name = "Paciente")]
        public Guid? IdPaciente { get; set; }

        [Required(ErrorMessage = "El odontólogo es obligatorio.")]
        [Display(Name = "Odontólogo")]
        public Guid? IdOdontologo { get; set; }

        [Display(Name = "Cita asociada (opcional)")]
        public Guid? IdCita { get; set; }

        [Display(Name = "Tipo de tratamiento")]
        public TipoTratamiento Tipo { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateOnly? Fecha { get; set; }

        [Display(Name = "Costo (Bs)")]
        public decimal Costo { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden tener más de 500 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
