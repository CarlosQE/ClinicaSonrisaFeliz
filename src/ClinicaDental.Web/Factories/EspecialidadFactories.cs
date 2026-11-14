using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class EspecialidadFactories
    {
        public static readonly Guid General = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Ortodoncia = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Endodoncia = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid Cirugia = Guid.Parse("10000000-0000-0000-0000-000000000004");

        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { Id = General, Nombre = "Odontología General", Descripcion = "Revisión, limpieza y tratamientos básicos" },
                new Especialidad { Id = Ortodoncia, Nombre = "Ortodoncia", Descripcion = "Corrección de la posición de los dientes" },
                new Especialidad { Id = Endodoncia, Nombre = "Endodoncia", Descripcion = "Tratamiento de conductos" },
                new Especialidad { Id = Cirugia, Nombre = "Cirugía Oral", Descripcion = "Extracciones y cirugías" }
            );
        }
    }
}
