using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.Enums
{
    public enum MetodoPago
    {
        [Display(Name = "Efectivo")] Efectivo = 0,
        [Display(Name = "Tarjeta")] Tarjeta = 1,
        [Display(Name = "Transferencia")] Transferencia = 2,
        [Display(Name = "QR")] QR = 3
    }
}
