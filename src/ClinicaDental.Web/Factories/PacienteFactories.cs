using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class PacienteFactories
    {
        public static readonly Guid Paciente1 = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid Paciente2 = Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid Paciente3 = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid Paciente4 = Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid Paciente5 = Guid.Parse("30000000-0000-0000-0000-000000000005");

        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>().HasData(
                new Paciente { Id = Paciente1, Nombres = "María Fernanda", Apellidos = "Quispe Mamani", CI = "7654321", FechaNacimiento = new DateOnly(1995, 3, 14), Telefono = "71111111", Email = "maria.quispe@correo.com", Direccion = "Av. Blanco Galindo km 4", FechaRegistro = new DateTime(2026, 11, 1) },
                new Paciente { Id = Paciente2, Nombres = "Juan Carlos", Apellidos = "Rojas Flores", CI = "8123456", FechaNacimiento = new DateOnly(1988, 7, 2), Telefono = "72222222", Email = null, Direccion = "Calle Bolívar #120", FechaRegistro = new DateTime(2026, 11, 1) },
                new Paciente { Id = Paciente3, Nombres = "Lucía", Apellidos = "Valverde Choque", CI = "9012345", FechaNacimiento = new DateOnly(2001, 11, 25), Telefono = "73333333", Email = "lucia.valverde@correo.com", Direccion = null, FechaRegistro = new DateTime(2026, 11, 2) },
                new Paciente { Id = Paciente4, Nombres = "Pedro Antonio", Apellidos = "Condori Vargas", CI = "6543210", FechaNacimiento = new DateOnly(1979, 1, 9), Telefono = "74444444", Email = null, Direccion = "Zona Sud, calle 5", FechaRegistro = new DateTime(2026, 11, 2) },
                new Paciente { Id = Paciente5, Nombres = "Sofía", Apellidos = "Mendoza Rivero", CI = "10234567", FechaNacimiento = new DateOnly(2010, 5, 30), Telefono = "75555555", Email = "sofia.mendoza@correo.com", Direccion = "Av. Circunvalación #45", FechaRegistro = new DateTime(2026, 11, 3) }
            );
        }
    }
}
