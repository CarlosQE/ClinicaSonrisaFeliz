using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class TratamientoFactories
{
    private const int CantidadBase = 2;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.tratamientos.Any()) return;

        context.Database.SetCommandTimeout(120);
        var faker = new Faker("es");
        var lote = new List<Tratamiento>(TamanoLote);

        var idCita1 = Guid.Parse("80000000-0000-0000-0000-000000000001");
        var idCita2 = Guid.Parse("80000000-0000-0000-0000-000000000002");

        // Los dos tratamientos históricos, ligados a sus citas atendidas.
        lote.AddRange(new[]
        {
            new Tratamiento
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                IdPaciente = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                IdOdontologo = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                IdCita = idCita1,
                Tipo = TipoTratamiento.LimpiezaDental,
                Fecha = new DateOnly(2026, 11, 5),
                Costo = 150.00m,
                Observaciones = "Limpieza y profilaxis completa"
            },
            new Tratamiento
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                IdPaciente = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                IdOdontologo = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                IdCita = idCita2,
                Tipo = TipoTratamiento.Endodoncia,
                Fecha = new DateOnly(2026, 11, 6),
                Costo = 800.00m,
                Observaciones = "Tratamiento de conducto, primera sesión"
            }
        });

        context.tratamientos.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        // Solo citas atendidas: garantizan paciente, odontólogo activo, fecha pasada y cita no cancelada.
        var citasDisponibles = context.citas
            .AsNoTracking()
            .Where(c => c.Estado == EstadoCita.Atendida && c.Id != idCita1 && c.Id != idCita2)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.Hora)
            .ThenBy(c => c.Id)
            .ToList();
        var extras = Math.Min(Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase), citasDisponibles.Count);

        for (var i = 0; i < extras; i++)
        {
            var cita = citasDisponibles[i];
            lote.Add(new Tratamiento
            {
                Id = Guid.NewGuid(),
                IdPaciente = cita.IdPaciente,
                IdOdontologo = cita.IdOdontologo,
                IdCita = cita.Id,
                Tipo = faker.PickRandom<TipoTratamiento>(),
                Fecha = cita.Fecha,
                Costo = Math.Round(faker.Random.Decimal(100, 1200), 2),
                Observaciones = Recortar(faker.Lorem.Sentence(4), 500)
            });

            if (lote.Count == TamanoLote)
            {
                context.tratamientos.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.tratamientos.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }

        static string Recortar(string valor, int longitud) =>
            valor.Length <= longitud ? valor : valor[..longitud];
    }
}
