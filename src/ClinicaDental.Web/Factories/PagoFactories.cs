using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class PagoFactories
{
    private const int CantidadBase = 1;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.pagos.Any()) return;

        context.Database.SetCommandTimeout(120);
        var faker = new Faker("es");
        var lote = new List<Pago>(TamanoLote);

        var idTratamientoBase = Guid.Parse("50000000-0000-0000-0000-000000000002");

        // Adelanto histórico documentado en el README.
        lote.Add(new Pago
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
            IdTratamiento = idTratamientoBase,
            Monto = 300.00m,
            FechaPago = new DateTime(2026, 11, 6, 12, 0, 0, DateTimeKind.Unspecified),
            Metodo = MetodoPago.Efectivo,
            NumeroComprobante = "CMP-000001",
            Observacion = "Adelanto"
        });

        context.pagos.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        // Un pago por tratamiento disponible; el tratamiento base ya tiene su adelanto.
        var tratamientosDisponibles = context.tratamientos
            .AsNoTracking()
            .Where(t => t.Id != idTratamientoBase)
            .OrderBy(t => t.Id)
            .ToList();
        var extras = Math.Min(Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase), tratamientosDisponibles.Count);

        for (var i = 0; i < extras; i++)
        {
            var tratamiento = tratamientosDisponibles[i];
            lote.Add(new Pago
            {
                Id = Guid.NewGuid(),
                IdTratamiento = tratamiento.Id,
                Monto = Math.Round(tratamiento.Costo * 0.4m, 2),
                FechaPago = tratamiento.Fecha.ToDateTime(new TimeOnly(12, 0)),
                Metodo = faker.PickRandom<MetodoPago>(),
                NumeroComprobante = $"{PagoServicio.PrefijoComprobante}{i + 2:D6}",
                Observacion = "Pago registrado en siembra"
            });

            if (lote.Count == TamanoLote)
            {
                context.pagos.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.pagos.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
    }
}
