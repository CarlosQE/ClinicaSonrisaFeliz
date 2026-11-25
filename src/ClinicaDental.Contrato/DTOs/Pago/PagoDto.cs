using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.Contrato.DTOs.Pago
{
    /// <summary>Cuerpo de POST /api/Pago.</summary>
    public class PagoDto
    {
        [Required(ErrorMessage = "El tratamiento es requerido")]
        [Display(Name = "Tratamiento")]
        public Guid? IdTratamiento { get; set; }

        [Display(Name = "Monto (Bs)")]
        public decimal Monto { get; set; }

        [Display(Name = "Método de pago")]
        public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;

        [StringLength(250, ErrorMessage = "La observación no puede tener más de 250 caracteres")]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }
    }
}
