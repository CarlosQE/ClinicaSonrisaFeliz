using ClinicaDental.Web.Models;

using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Pago
{
    public class PagoDto
    {
        [Required(ErrorMessage = "El tratamiento es requerido")]
        public Guid IdTratamiento { get; set; }

        // Reglas (mayor a 0, máximo 2 decimales, no mayor al saldo) se validan en el servicio.
        public decimal Monto { get; set; }

        // 0 = Efectivo, 1 = Tarjeta, 2 = Transferencia, 3 = QR
        public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;

        [MaxLength(250, ErrorMessage = "La observación no puede tener más de 250 caracteres")]
        public string? Observacion { get; set; }
    }
}