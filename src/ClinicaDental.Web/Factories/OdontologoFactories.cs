
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class OdontologoFactories
    {
        public static readonly Guid Odo1 = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Odo2 = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Odo3 = Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Odo4 = Guid.Parse("20000000-0000-0000-0000-000000000004");

        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Odontologo>().HasData(
                new Odontologo { Id = Odo1, Nombres = "Ricardo", Apellidos = "Salinas Pérez", Matricula = "MP-0001", Telefono = "70000001", Email = "ricardo.salinas@sonrisafeliz.bo", Activo = true, IdEspecialidad = EspecialidadFactories.General },
                new Odontologo { Id = Odo2, Nombres = "Valeria", Apellidos = "Camacho Ortiz", Matricula = "MP-0002", Telefono = "70000002", Email = "valeria.camacho@sonrisafeliz.bo", Activo = true, IdEspecialidad = EspecialidadFactories.Ortodoncia },
                new Odontologo { Id = Odo3, Nombres = "Marcelo", Apellidos = "Gutiérrez Soria", Matricula = "MP-0003", Telefono = "70000003", Email = "marcelo.gutierrez@sonrisafeliz.bo", Activo = true, IdEspecialidad = EspecialidadFactories.Endodoncia },
                new Odontologo { Id = Odo4, Nombres = "Patricia", Apellidos = "Lozano Rojas", Matricula = "MP-0004", Telefono = "70000004", Email = null, Activo = false, IdEspecialidad = EspecialidadFactories.Cirugia }
            );
        }
    }
}
