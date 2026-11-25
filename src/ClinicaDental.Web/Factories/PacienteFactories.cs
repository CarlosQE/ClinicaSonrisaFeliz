using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class PacienteFactories
{
    private const int CantidadBase = 5;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.pacientes.Any()) return;

        context.Database.SetCommandTimeout(120);
        var extras = Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase);
        var faker = new Faker("es");
        var lote = new List<Paciente>(TamanoLote);

        // Cinco pacientes históricos con sus ids, CI, correos y fechas originales.
        lote.AddRange(new[]
        {
            new Paciente
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Nombres = "María Fernanda",
                Apellidos = "Quispe Mamani",
                CI = "7654321",
                Direccion = "Av. Blanco Galindo km 4",
                Email = "maria.quispe@correo.com",
                FechaNacimiento = new DateOnly(1995, 3, 14),
                FechaRegistro = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Telefono = "71111111"
            },
            new Paciente
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Nombres = "Juan Carlos",
                Apellidos = "Rojas Flores",
                CI = "8123456",
                Direccion = "Calle Bolívar #120",
                Email = null,
                FechaNacimiento = new DateOnly(1988, 7, 2),
                FechaRegistro = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Telefono = "72222222"
            },
            new Paciente
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                Nombres = "Lucía",
                Apellidos = "Valverde Choque",
                CI = "9012345",
                Direccion = null,
                Email = "lucia.valverde@correo.com",
                FechaNacimiento = new DateOnly(2001, 11, 25),
                FechaRegistro = new DateTime(2026, 11, 2, 0, 0, 0, DateTimeKind.Unspecified),
                Telefono = "73333333"
            },
            new Paciente
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                Nombres = "Pedro Antonio",
                Apellidos = "Condori Vargas",
                CI = "6543210",
                Direccion = "Zona Sud, calle 5",
                Email = null,
                FechaNacimiento = new DateOnly(1979, 1, 9),
                FechaRegistro = new DateTime(2026, 11, 2, 0, 0, 0, DateTimeKind.Unspecified),
                Telefono = "74444444"
            },
            new Paciente
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000005"),
                Nombres = "Sofía",
                Apellidos = "Mendoza Rivero",
                CI = "10234567",
                Direccion = "Av. Circunvalación #45",
                Email = "sofia.mendoza@correo.com",
                FechaNacimiento = new DateOnly(2010, 5, 30),
                FechaRegistro = new DateTime(2026, 11, 3, 0, 0, 0, DateTimeKind.Unspecified),
                Telefono = "75555555"
            }
        });

        // Nombres completos ya usados: los extras nunca repiten uno de estos ni entre sí.
        var usados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in lote)
        {
            usados.Add($"{p.Nombres} {p.Apellidos}");
        }

        context.pacientes.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        var nombres = new[]
        {
            "Ana", "Carlos", "Diego", "Elena", "Fernando", "Gabriela", "Isabel",
            "Javier", "Lucía", "María", "Nicolás", "Patricia", "Ricardo", "Sofía", "Tamara",
            "Alejandro", "Andrea", "Bruno", "Camila", "Daniela", "Eduardo", "Fabiola",
            "Gonzalo", "Héctor", "Ingrid", "Jorge", "Karen", "Luis", "Marcelo", "Natalia",
            "Óscar", "Paola", "Rodrigo", "Silvia", "Tomás", "Valeria", "Wilson", "Ximena",
            "Yolanda"
        };
        var apellidos = new[]
        {
            "Apaza", "Cáceres", "Choque", "Condori", "Espinoza", "Flores", "García",
            "Huanca", "Jiménez", "Mamani", "Mendoza", "Pérez", "Quispe", "Rojas", "Vargas",
            "Aguilar", "Álvarez", "Arce", "Balderrama", "Benítez", "Calizaya", "Castro",
            "Cruz", "Fernández", "Gutiérrez", "Herrera", "Lima", "López", "Medina",
            "Montaño", "Navarro", "Ortiz", "Paz", "Ramos", "Salazar", "Soria", "Torrez",
            "Velasco", "Zapata", "Rivero"
        };
        var limiteNacimiento = DateTime.Today.AddYears(-18);

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

            lote.Add(new Paciente
            {
                Id = Guid.NewGuid(),
                Nombres = nombre,
                Apellidos = apellido,
                CI = $"{70000000 + i:D8}",
                Direccion = Recortar(faker.Address.FullAddress(), 150),
                Email = $"paciente{numero:D6}@ejemplo.bo",
                FechaNacimiento = DateOnly.FromDateTime(faker.Date.Between(new DateTime(1960, 1, 1), limiteNacimiento)),
                FechaRegistro = DateTime.UtcNow,
                Telefono = faker.Phone.PhoneNumber("7#######")
            });

            if (lote.Count == TamanoLote)
            {
                context.pacientes.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.pacientes.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }

        static string Recortar(string valor, int longitud) =>
            valor.Length <= longitud ? valor : valor[..longitud];
    }
}