using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class OdontologoFactories
{
    private const int CantidadBase = 4;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.odontologos.Any()) return;

        context.Database.SetCommandTimeout(120);
        var extras = Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase);
        var faker = new Faker("es");
        var lote = new List<Odontologo>(TamanoLote);

        // Matrículas normalizadas sin guion para cumplir MatriculaValida; ids y datos históricos se conservan.
        lote.AddRange(new[]
        {
            new Odontologo
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Nombres = "Ricardo",
                Apellidos = "Salinas Pérez",
                Matricula = "MP0001",
                Telefono = "70000001",
                Email = "ricardo.salinas@sonrisafeliz.bo",
                Activo = true,
                IdEspecialidad = Guid.Parse("10000000-0000-0000-0000-000000000001")
            },
            new Odontologo
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Nombres = "Valeria",
                Apellidos = "Camacho Ortiz",
                Matricula = "MP0002",
                Telefono = "70000002",
                Email = "valeria.camacho@sonrisafeliz.bo",
                Activo = true,
                IdEspecialidad = Guid.Parse("10000000-0000-0000-0000-000000000002")
            },
            new Odontologo
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Nombres = "Marcelo",
                Apellidos = "Gutiérrez Soria",
                Matricula = "MP0003",
                Telefono = "70000003",
                Email = "marcelo.gutierrez@sonrisafeliz.bo",
                Activo = true,
                IdEspecialidad = Guid.Parse("10000000-0000-0000-0000-000000000003")
            },
            new Odontologo
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                Nombres = "Patricia",
                Apellidos = "Lozano Rojas",
                Matricula = "MP0004",
                Telefono = "70000004",
                Email = null,
                Activo = false,
                IdEspecialidad = Guid.Parse("10000000-0000-0000-0000-000000000004")
            }
        });

        context.odontologos.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        var especialidades = context.especialidades.Select(e => e.Id).ToList();
        for (var i = 1; i <= extras; i++)
        {
            var numero = i + CantidadBase;
            lote.Add(new Odontologo
            {
                Id = Guid.NewGuid(),
                Nombres = faker.Name.FirstName(),
                Apellidos = faker.Name.LastName(),
                Matricula = $"MP{numero:D6}",
                Telefono = faker.Phone.PhoneNumber("79########"),
                Email = $"odontologo{numero:D6}@ejemplo.bo",
                Activo = true,
                IdEspecialidad = especialidades[(i - 1) % especialidades.Count]
            });

            if (lote.Count == TamanoLote)
            {
                context.odontologos.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.odontologos.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
    }
}
