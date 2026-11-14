using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Data
{
    public static class DatosIniciales
    {
        public static void Sembrar(ModelBuilder modelBuilder)
        {
            var espGeneral = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var espOrtodoncia = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var espEndodoncia = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var espCirugia = Guid.Parse("10000000-0000-0000-0000-000000000004");

            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { Id = espGeneral, Nombre = "Odontología General", Descripcion = "Revisión, limpieza y tratamientos básicos" },
                new Especialidad { Id = espOrtodoncia, Nombre = "Ortodoncia", Descripcion = "Corrección de la posición de los dientes" },
                new Especialidad { Id = espEndodoncia, Nombre = "Endodoncia", Descripcion = "Tratamiento de conductos" },
                new Especialidad { Id = espCirugia, Nombre = "Cirugía Oral", Descripcion = "Extracciones y cirugías" }
            );

            var odo1 = Guid.Parse("20000000-0000-0000-0000-000000000001");
            var odo2 = Guid.Parse("20000000-0000-0000-0000-000000000002");
            var odo3 = Guid.Parse("20000000-0000-0000-0000-000000000003");
            var odo4 = Guid.Parse("20000000-0000-0000-0000-000000000004");

            modelBuilder.Entity<Odontologo>().HasData(
                new Odontologo { Id = odo1, Nombres = "Ricardo", Apellidos = "Salinas Pérez", Matricula = "MP-0001", Telefono = "70000001", Email = "ricardo.salinas@sonrisafeliz.bo", Activo = true, IdEspecialidad = espGeneral },
                new Odontologo { Id = odo2, Nombres = "Valeria", Apellidos = "Camacho Ortiz", Matricula = "MP-0002", Telefono = "70000002", Email = "valeria.camacho@sonrisafeliz.bo", Activo = true, IdEspecialidad = espOrtodoncia },
                new Odontologo { Id = odo3, Nombres = "Marcelo", Apellidos = "Gutiérrez Soria", Matricula = "MP-0003", Telefono = "70000003", Email = "marcelo.gutierrez@sonrisafeliz.bo", Activo = true, IdEspecialidad = espEndodoncia },
                new Odontologo { Id = odo4, Nombres = "Patricia", Apellidos = "Lozano Rojas", Matricula = "MP-0004", Telefono = "70000004", Email = null, Activo = false, IdEspecialidad = espCirugia }
            );

            // Horarios: lunes a viernes, 08:00-12:00 y 14:00-18:00, para los 4 odontólogos.
            // Los Guids son deterministas (HasData exige valores fijos).
            var horarios = new List<HorarioOdontologo>();
            var numero = 1;
            foreach (var idOdontologo in new[] { odo1, odo2, odo3, odo4 })
            {
                foreach (var dia in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                {
                    horarios.Add(new HorarioOdontologo
                    {
                        Id = Guid.Parse($"40000000-0000-0000-0000-{numero++:D12}"),
                        IdOdontologo = idOdontologo,
                        DiaSemana = dia,
                        HoraInicio = new TimeOnly(8, 0),
                        HoraFin = new TimeOnly(12, 0)
                    });
                    horarios.Add(new HorarioOdontologo
                    {
                        Id = Guid.Parse($"40000000-0000-0000-0000-{numero++:D12}"),
                        IdOdontologo = idOdontologo,
                        DiaSemana = dia,
                        HoraInicio = new TimeOnly(14, 0),
                        HoraFin = new TimeOnly(18, 0)
                    });
                }
            }
            modelBuilder.Entity<HorarioOdontologo>().HasData(horarios);

            modelBuilder.Entity<Paciente>().HasData(
                new Paciente { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Nombres = "María Fernanda", Apellidos = "Quispe Mamani", CI = "7654321", FechaNacimiento = new DateOnly(1995, 3, 14), Telefono = "71111111", Email = "maria.quispe@correo.com", Direccion = "Av. Blanco Galindo km 4", FechaRegistro = new DateTime(2026, 11, 1) },
                new Paciente { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Nombres = "Juan Carlos", Apellidos = "Rojas Flores", CI = "8123456", FechaNacimiento = new DateOnly(1988, 7, 2), Telefono = "72222222", Email = null, Direccion = "Calle Bolívar #120", FechaRegistro = new DateTime(2026, 11, 1) },
                new Paciente { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Nombres = "Lucía", Apellidos = "Valverde Choque", CI = "9012345", FechaNacimiento = new DateOnly(2001, 11, 25), Telefono = "73333333", Email = "lucia.valverde@correo.com", Direccion = null, FechaRegistro = new DateTime(2026, 11, 2) },
                new Paciente { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Nombres = "Pedro Antonio", Apellidos = "Condori Vargas", CI = "6543210", FechaNacimiento = new DateOnly(1979, 1, 9), Telefono = "74444444", Email = null, Direccion = "Zona Sud, calle 5", FechaRegistro = new DateTime(2026, 11, 2) },
                new Paciente { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), Nombres = "Sofía", Apellidos = "Mendoza Rivero", CI = "10234567", FechaNacimiento = new DateOnly(2010, 5, 30), Telefono = "75555555", Email = "sofia.mendoza@correo.com", Direccion = "Av. Circunvalación #45", FechaRegistro = new DateTime(2026, 11, 3) }
            );
        }
    }
}
