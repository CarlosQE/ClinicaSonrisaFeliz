using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class CitaFactories
    {
        public static readonly Guid Cita1 = Guid.Parse("80000000-0000-0000-0000-000000000001");
        public static readonly Guid Cita2 = Guid.Parse("80000000-0000-0000-0000-000000000002");
        public static readonly Guid Cita3 = Guid.Parse("80000000-0000-0000-0000-000000000003");
        public static readonly Guid Cita4 = Guid.Parse("80000000-0000-0000-0000-000000000004");
        public static readonly Guid Cita5 = Guid.Parse("80000000-0000-0000-0000-000000000005");
        public static readonly Guid Cita6 = Guid.Parse("80000000-0000-0000-0000-000000000006");
        public static readonly Guid Cita7 = Guid.Parse("80000000-0000-0000-0000-000000000007");

        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cita>().HasData(
                new Cita { Id = Cita1, IdPaciente = PacienteFactories.Paciente1, IdOdontologo = OdontologoFactories.Odo1, Fecha = new DateOnly(2026, 11, 5), Hora = new TimeOnly(9, 0), DuracionMinutos = 30, Estado = EstadoCita.Atendida, Motivo = "Limpieza dental", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 3, 10, 0, 0) },
                new Cita { Id = Cita2, IdPaciente = PacienteFactories.Paciente2, IdOdontologo = OdontologoFactories.Odo3, Fecha = new DateOnly(2026, 11, 6), Hora = new TimeOnly(10, 0), DuracionMinutos = 60, Estado = EstadoCita.Atendida, Motivo = "Dolor de muela, posible conducto", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 4, 11, 0, 0) },
                new Cita { Id = Cita3, IdPaciente = PacienteFactories.Paciente4, IdOdontologo = OdontologoFactories.Odo3, Fecha = new DateOnly(2026, 11, 10), Hora = new TimeOnly(14, 0), DuracionMinutos = 30, Estado = EstadoCita.Cancelada, Motivo = "Control de endodoncia", MotivoCancelacion = "El paciente no pudo asistir", FechaRegistro = new DateTime(2026, 11, 5, 9, 0, 0) },
                new Cita { Id = Cita4, IdPaciente = PacienteFactories.Paciente3, IdOdontologo = OdontologoFactories.Odo2, Fecha = new DateOnly(2026, 12, 1), Hora = new TimeOnly(10, 0), DuracionMinutos = 30, Estado = EstadoCita.Programada, Motivo = "Control de ortodoncia", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 20, 15, 0, 0) },
                new Cita { Id = Cita5, IdPaciente = PacienteFactories.Paciente4, IdOdontologo = OdontologoFactories.Odo1, Fecha = new DateOnly(2026, 12, 2), Hora = new TimeOnly(15, 0), DuracionMinutos = 30, Estado = EstadoCita.Programada, Motivo = "Revisión general", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 20, 16, 0, 0) },
                new Cita { Id = Cita6, IdPaciente = PacienteFactories.Paciente5, IdOdontologo = OdontologoFactories.Odo2, Fecha = new DateOnly(2026, 12, 3), Hora = new TimeOnly(9, 30), DuracionMinutos = 30, Estado = EstadoCita.Programada, Motivo = "Primera consulta de ortodoncia", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 21, 10, 0, 0) },
                new Cita { Id = Cita7, IdPaciente = PacienteFactories.Paciente3, IdOdontologo = OdontologoFactories.Odo1, Fecha = new DateOnly(2026, 12, 4), Hora = new TimeOnly(16, 0), DuracionMinutos = 30, Estado = EstadoCita.Reprogramada, Motivo = "Limpieza dental", MotivoCancelacion = null, FechaRegistro = new DateTime(2026, 11, 21, 12, 0, 0) }
            );
        }
    }
}
