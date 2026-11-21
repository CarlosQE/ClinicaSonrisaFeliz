using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.Contrato.DTOs
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

        [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "El costo debe ser mayor a 0.")]
        [Display(Name = "Costo (Bs)")]
        public decimal Costo { get; set; }

        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }

    /// <summary>Cuerpo de PUT /api/Tratamiento/{id}/observaciones.</summary>
    public class ActualizarObservacionesDto
    {
        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres.")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;
    }

    /// <summary>Cuerpo de POST /api/Pago.</summary>
    public class PagoDto
    {
        [Required(ErrorMessage = "El tratamiento es requerido")]
        [Display(Name = "Tratamiento")]
        public Guid? IdTratamiento { get; set; }

        [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "El monto debe ser mayor a 0.")]
        [Display(Name = "Monto (Bs)")]
        public decimal Monto { get; set; }

        [Display(Name = "Método de pago")]
        public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;

        [MaxLength(250, ErrorMessage = "La observación no puede tener más de 250 caracteres")]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }
    }
}
