using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaDental.Web.Models
{
    public class HorarioOdontologo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdOdontologo { get; set; }
        [ForeignKey(nameof(IdOdontologo))]
        public virtual Odontologo? Odontologo { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }
}
