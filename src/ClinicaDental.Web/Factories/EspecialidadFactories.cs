using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class EspecialidadFactories
{
    private const int CantidadBase = 10;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.especialidades.Any()) return;

        context.Database.SetCommandTimeout(120);
        var extras = Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase);
        var faker = new Faker("es");
        var lote = new List<Especialidad>(TamanoLote);

        // Catálogo base: los cuatro ids históricos y seis especialidades adicionales con ids fijos.
        lote.AddRange(new[]
        {
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Nombre = "Odontología General",
                Descripcion = "Revisión, limpieza y tratamientos básicos"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Nombre = "Ortodoncia",
                Descripcion = "Corrección de la posición de los dientes"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Nombre = "Endodoncia",
                Descripcion = "Tratamiento de conductos"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Nombre = "Cirugía Oral",
                Descripcion = "Extracciones y cirugías"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Nombre = "Periodoncia",
                Descripcion = "Tratamiento de las encías y tejidos de soporte"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Nombre = "Prostodoncia",
                Descripcion = "Rehabilitación con prótesis dentales"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                Nombre = "Pedodontía",
                Descripcion = "Atención odontológica infantil"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                Nombre = "Implantología",
                Descripcion = "Colocación de implantes dentales"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                Nombre = "Ortopedia Maxilar",
                Descripcion = "Corrección del crecimiento óseo facial"
            },
            new Especialidad
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000010"),
                Nombre = "Medicina Oral",
                Descripcion = "Diagnóstico de enfermedades de la cavidad oral"
            }
        });

        context.especialidades.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        for (var i = 1; i <= extras; i++)
        {
            lote.Add(new Especialidad
            {
                Id = Guid.NewGuid(),
                Nombre = NombreExtra(i),
                Descripcion = Recortar(faker.Lorem.Sentence(), 200)
            });

            if (lote.Count == TamanoLote)
            {
                context.especialidades.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.especialidades.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }

        static string NombreExtra(int numero)
        {
            var letras = string.Empty;
            do
            {
                var resto = (numero - 1) % 26;
                letras = (char)('A' + resto) + letras;
                numero = (numero - 1) / 26;
            } while (numero > 0);

            return $"Especialidad {letras}";
        }

        static string Recortar(string valor, int longitud) =>
            valor.Length <= longitud ? valor : valor[..longitud];
    }
}
