using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class TratamientoFactories
    {
        public static readonly Guid Trat1 = Guid.Parse("50000000-0000-0000-0000-000000000001");
        public static readonly Guid Trat2 = Guid.Parse("50000000-0000-0000-0000-000000000002");

        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tratamiento>().HasData(
                new Tratamiento { Id = Trat1, IdPaciente = PacienteFactories.Paciente1, IdOdontologo = OdontologoFactories.Odo1, IdCita = CitaFactories.Cita1, Tipo = TipoTratamiento.LimpiezaDental, Fecha = new DateOnly(2026, 11, 5), Costo = 150.00m, Observaciones = "Limpieza y profilaxis completa" },
                new Tratamiento { Id = Trat2, IdPaciente = PacienteFactories.Paciente2, IdOdontologo = OdontologoFactories.Odo3, IdCita = CitaFactories.Cita2, Tipo = TipoTratamiento.Endodoncia, Fecha = new DateOnly(2026, 11, 6), Costo = 800.00m, Observaciones = "Tratamiento de conducto, primera sesión" }
            );
        }
    }
}
