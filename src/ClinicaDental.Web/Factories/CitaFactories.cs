using Bogus;

using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class CitaFactories
{
    private const int CantidadBase = 7;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.citas.Any()) return;

        context.Database.SetCommandTimeout(120);
        var extras = Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase);
        var faker = new Faker("es");
        var lote = new List<Cita>(TamanoLote);

        var idPaciente1 = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var idPaciente2 = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var idPaciente3 = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var idPaciente4 = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var idPaciente5 = Guid.Parse("30000000-0000-0000-0000-000000000005");
        var idOdontologo1 = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var idOdontologo2 = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var idOdontologo3 = Guid.Parse("20000000-0000-0000-0000-000000000003");

        // Siete citas del catálogo: dos atendidas, una cancelada, tres programadas y una reprogramada.
        lote.AddRange(new[]
        {
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                IdPaciente = idPaciente1,
                IdOdontologo = idOdontologo1,
                Fecha = new DateOnly(2026, 11, 5),
                Hora = new TimeOnly(9, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Atendida,
                Motivo = "Limpieza dental",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                IdPaciente = idPaciente2,
                IdOdontologo = idOdontologo3,
                Fecha = new DateOnly(2026, 11, 6),
                Hora = new TimeOnly(10, 0, 0),
                DuracionMinutos = 60,
                Estado = EstadoCita.Atendida,
                Motivo = "Dolor de muela, posible conducto",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 4, 11, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                IdPaciente = idPaciente4,
                IdOdontologo = idOdontologo3,
                Fecha = new DateOnly(2026, 11, 10),
                Hora = new TimeOnly(14, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Cancelada,
                Motivo = "Control de endodoncia",
                MotivoCancelacion = "El paciente no pudo asistir",
                FechaRegistro = new DateTime(2026, 11, 5, 9, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                IdPaciente = idPaciente3,
                IdOdontologo = idOdontologo2,
                Fecha = new DateOnly(2026, 12, 1),
                Hora = new TimeOnly(10, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Programada,
                Motivo = "Control de ortodoncia",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 20, 15, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                IdPaciente = idPaciente4,
                IdOdontologo = idOdontologo1,
                Fecha = new DateOnly(2026, 12, 2),
                Hora = new TimeOnly(15, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Programada,
                Motivo = "Revisión general",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 20, 16, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000006"),
                IdPaciente = idPaciente5,
                IdOdontologo = idOdontologo2,
                Fecha = new DateOnly(2026, 12, 3),
                Hora = new TimeOnly(9, 30, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Programada,
                Motivo = "Primera consulta de ortodoncia",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 21, 10, 0, 0, DateTimeKind.Unspecified)
            },
            new Cita
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000007"),
                IdPaciente = idPaciente3,
                IdOdontologo = idOdontologo1,
                Fecha = new DateOnly(2026, 12, 4),
                Hora = new TimeOnly(16, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Reprogramada,
                Motivo = "Limpieza dental",
                MotivoCancelacion = null,
                FechaRegistro = new DateTime(2026, 11, 21, 12, 0, 0, DateTimeKind.Unspecified)
            }
        });

        context.citas.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        var horarios = context.horariosOdontologo.AsNoTracking().ToList();
        var odontologos = context.odontologos
            .Where(o => o.Activo)
            .OrderBy(o => o.Matricula)
            .ToList()
            .Where(o => horarios.Any(h => h.IdOdontologo == o.Id))
            .Select(o => o.Id)
            .ToList();
        var horariosPorOdontologo = horarios
            .GroupBy(h => h.IdOdontologo)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio).ToList());
        var pacientes = context.pacientes
            .OrderBy(p => p.CI)
            .Select(p => p.Id)
            .ToList();
        var motivos = new[]
        {
            "Limpieza dental", "Control de ortodoncia", "Revisión general",
            "Tratamiento de conducto", "Profilaxis", "Extracción programada"
        };
        var hoy = DateOnly.FromDateTime(DateTime.Today);

        // Las citas extra son históricas (Atendida): así no exigen rango futuro y admiten tratamientos.
        for (var i = 1; i <= extras; i++)
        {
            var idOdontologo = odontologos[(i - 1) % odontologos.Count];
            var ciclo = (i - 1) / odontologos.Count;
            var horariosDelOdontologo = horariosPorOdontologo[idOdontologo];
            var horario = horariosDelOdontologo[ciclo % horariosDelOdontologo.Count];
            var fecha = hoy;
            while (fecha >= hoy || fecha.DayOfWeek != horario.DiaSemana)
            {
                fecha = fecha.AddDays(-1);
            }

            var hora = horario.HoraInicio.AddMinutes(60 + 60 * (ciclo / horariosDelOdontologo.Count));
            lote.Add(new Cita
            {
                Id = Guid.NewGuid(),
                IdPaciente = pacientes[(i - 1) % pacientes.Count],
                IdOdontologo = idOdontologo,
                Fecha = fecha,
                Hora = hora,
                DuracionMinutos = 30,
                Estado = EstadoCita.Atendida,
                Motivo = faker.PickRandom(motivos),
                MotivoCancelacion = null,
                FechaRegistro = fecha.ToDateTime(hora).AddDays(-1)
            });

            if (lote.Count == TamanoLote)
            {
                context.citas.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.citas.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
    }
}
