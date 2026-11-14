using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaDental.Web.Models
{
    [Index(nameof(NumeroComprobante), IsUnique = true)]
    public class Pago
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdTratamiento { get; set; }
        [ForeignKey(nameof(IdTratamiento))]
        public virtual Tratamiento? Tratamiento { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago Metodo { get; set; }
        [MaxLength(20)]
        public string NumeroComprobante { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Observacion { get; set; }
    }
}
