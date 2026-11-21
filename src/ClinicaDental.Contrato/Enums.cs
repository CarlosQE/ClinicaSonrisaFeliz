using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.Enums
{
    // Mismos valores enteros que ClinicaDental.Web.Models (la API los envía y recibe como int).

    public enum EstadoCita
    {
        Programada = 0,
        Reprogramada = 1,
        Cancelada = 2,
        Atendida = 3
    }

    public enum TipoTratamiento
    {
        [Display(Name = "Limpieza dental")] LimpiezaDental = 0,
        [Display(Name = "Ortodoncia")] Ortodoncia = 1,
        [Display(Name = "Endodoncia")] Endodoncia = 2,
        [Display(Name = "Extracción")] Extraccion = 3
    }

    public enum MetodoPago
    {
        [Display(Name = "Efectivo")] Efectivo = 0,
        [Display(Name = "Tarjeta")] Tarjeta = 1,
        [Display(Name = "Transferencia")] Transferencia = 2,
        [Display(Name = "QR")] QR = 3
    }
}
