using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class EspecialidadFactories
{
    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.especialidades.Any()) return;

        context.Database.SetCommandTimeout(120);

        // Catálogo real de especialidades. No se generan extras: el parámetro se conserva
        // para no cambiar la llamada desde DatosIniciales.
        var lote = new List<Especialidad>
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
        };

        context.especialidades.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}