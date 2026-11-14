using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaDental.Web.Models
{
    public class Tratamiento
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public virtual Paciente? Paciente { get; set; }
        public Guid IdOdontologo { get; set; }
        [ForeignKey(nameof(IdOdontologo))]
        public virtual Odontologo? Odontologo { get; set; }
        public Guid? IdCita { get; set; }
        [ForeignKey(nameof(IdCita))]
        public virtual Cita? Cita { get; set; }
        public TipoTratamiento Tipo { get; set; }
        public DateOnly Fecha { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }
        [MaxLength(1000)]
        public string? Observaciones { get; set; }
    }
}
