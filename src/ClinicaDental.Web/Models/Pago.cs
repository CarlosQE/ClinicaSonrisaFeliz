using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models;

/// <summary>
/// Pago (total o parcial) de un tratamiento. Dueño funcional: módulo 5 (Pagos).
/// CONTRATO: las propiedades existentes no se renombran ni se eliminan.
/// </summary>
public class Pago
{
    public int Id { get; set; }

    [Display(Name = "Tratamiento")]
    public int TratamientoId { get; set; }
    public Tratamiento? Tratamiento { get; set; }

    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "El monto debe ser mayor a 0.")]
    public decimal Monto { get; set; }

    [Display(Name = "Fecha de pago")]
    public DateTime FechaPago { get; set; }

    [Display(Name = "Método de pago")]
    public MetodoPago Metodo { get; set; }

    /// <summary>Número de comprobante, lo genera el servicio. Único. Formato: CMP-000001.</summary>
    [StringLength(20)]
    [Display(Name = "N° comprobante")]
    public string NumeroComprobante { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Observacion { get; set; }
}
