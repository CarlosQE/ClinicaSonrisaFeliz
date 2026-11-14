using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class HorarioOdontologoFactories
    {
        public static void Sembrar(ModelBuilder modelBuilder)
        {
            // Horarios: lunes a viernes, 08:00-12:00 y 14:00-18:00, para los 4 odontólogos.
            // Los Guids son deterministas (HasData exige valores fijos).
            var horarios = new List<HorarioOdontologo>();
            var numero = 1;
            foreach (var idOdontologo in new[] { OdontologoFactories.Odo1, OdontologoFactories.Odo2, OdontologoFactories.Odo3, OdontologoFactories.Odo4 })
            {
                foreach (var dia in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                {
                    horarios.Add(new HorarioOdontologo
                    {
                        Id = Guid.Parse($"40000000-0000-0000-0000-{numero++:D12}"),
                        IdOdontologo = idOdontologo,
                        DiaSemana = dia,
                        HoraInicio = new TimeOnly(8, 0),
                        HoraFin = new TimeOnly(12, 0)
                    });
                    horarios.Add(new HorarioOdontologo
                    {
                        Id = Guid.Parse($"40000000-0000-0000-0000-{numero++:D12}"),
                        IdOdontologo = idOdontologo,
                        DiaSemana = dia,
                        HoraInicio = new TimeOnly(14, 0),
                        HoraFin = new TimeOnly(18, 0)
                    });
                }
            }
            modelBuilder.Entity<HorarioOdontologo>().HasData(horarios);
        }
    }
}
