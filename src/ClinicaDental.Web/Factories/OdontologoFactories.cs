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
        var extras = Math.Clamp(cantidadExtra, 0, 100 - CantidadBase);
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

        // Nombres completos ya usados: los extras nunca repiten uno de estos ni entre sí.
        var usados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var o in lote)
        {
            usados.Add($"{o.Nombres} {o.Apellidos}");
        }

        context.odontologos.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        var nombres = new[]
        {
            "Ana", "Carlos", "Diego", "Elena", "Fernando", "Gabriela", "Isabel",
            "Javier", "Lucía", "María", "Nicolás", "Ricardo", "Sofía", "Tamara",
            "Alejandro", "Andrea", "Bruno", "Camila", "Daniela", "Eduardo", "Fabiola",
            "Gonzalo", "Héctor", "Ingrid", "Jorge", "Karen", "Luis", "Natalia",
            "Óscar", "Paola", "Rodrigo", "Silvia", "Tomás", "Ximena", "Yolanda"
        };
        var apellidos = new[]
        {
            "Apaza", "Cáceres", "Choque", "Condori", "Espinoza", "Flores", "García",
            "Huanca", "Jiménez", "Mamani", "Mendoza", "Pérez", "Quispe", "Rojas", "Vargas",
            "Aguilar", "Álvarez", "Arce", "Balderrama", "Benítez", "Calizaya", "Castro",
            "Cruz", "Fernández", "Herrera", "Lima", "López", "Medina", "Montaño",
            "Navarro", "Ortiz", "Paz", "Ramos", "Salazar", "Torrez", "Velasco", "Zapata"
        };

        var especialidades = context.especialidades.Select(e => e.Id).ToList();
        for (var i = 1; i <= extras; i++)
        {
            var numero = i + CantidadBase;

            string nombre;
            string apellido;
            while (true)
            {
                nombre = faker.PickRandom(nombres);
                var segundo = faker.PickRandom(nombres);
                if (faker.Random.Bool() && segundo != nombre)
                {
                    nombre = $"{nombre} {segundo}";
                }

                var paterno = faker.PickRandom(apellidos);
                var materno = faker.PickRandom(apellidos);
                if (paterno == materno) continue;

                apellido = $"{paterno} {materno}";
                if (usados.Add($"{nombre} {apellido}")) break;
            }

            lote.Add(new Odontologo
            {
                Id = Guid.NewGuid(),
                Nombres = nombre,
                Apellidos = apellido,
                Matricula = $"MP{numero:D6}",
                Telefono = faker.Phone.PhoneNumber("7#######"),
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