using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Data;

/// <summary>
/// Datos de ejemplo para que TODOS los grupos puedan probar su módulo desde el primer día,
/// aunque los módulos de los que dependen todavía no estén terminados.
/// Solo se cargan si la base está vacía. Dueño: grupo líder.
/// </summary>
public static class DatosIniciales
{
    public static void Sembrar(ClinicaDbContext db, TimeProvider reloj)
    {
        if (db.Especialidades.Any()) return;

        var hoy = DateOnly.FromDateTime(reloj.GetLocalNow().DateTime);

        // ---------- Especialidades ----------
        var general = new Especialidad { Nombre = "Odontología General", Descripcion = "Diagnóstico y tratamientos básicos." };
        var orto = new Especialidad { Nombre = "Ortodoncia", Descripcion = "Corrección de la posición de dientes y maxilares." };
        var endo = new Especialidad { Nombre = "Endodoncia", Descripcion = "Tratamiento de conductos." };
        var cirugia = new Especialidad { Nombre = "Cirugía Oral", Descripcion = "Extracciones y cirugías menores." };
        db.Especialidades.AddRange(general, orto, endo, cirugia);

        // ---------- Odontólogos + horarios ----------
        var laboral = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

        var ana = new Odontologo { Nombres = "Ana", Apellidos = "Rojas Vaca", Matricula = "MAT-1001", Telefono = "70000001", Email = "ana.rojas@sonrisafeliz.bo", Especialidad = general };
        var luis = new Odontologo { Nombres = "Luis", Apellidos = "Mendoza Paz", Matricula = "MAT-1002", Telefono = "70000002", Email = "luis.mendoza@sonrisafeliz.bo", Especialidad = orto };
        var carla = new Odontologo { Nombres = "Carla", Apellidos = "Suárez Gil", Matricula = "MAT-1003", Telefono = "70000003", Especialidad = endo };
        var pedro = new Odontologo { Nombres = "Pedro", Apellidos = "Flores Ríos", Matricula = "MAT-1004", Telefono = "70000004", Especialidad = cirugia, Activo = false };

        foreach (var dia in laboral)
        {
            ana.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(8, 0), HoraFin = new(12, 0) });
            ana.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(14, 0), HoraFin = new(18, 0) });
            luis.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(14, 0), HoraFin = new(20, 0) });
            pedro.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(8, 0), HoraFin = new(12, 0) });
        }
        carla.Horarios.Add(new HorarioOdontologo { DiaSemana = DayOfWeek.Saturday, HoraInicio = new(8, 0), HoraFin = new(13, 0) });
        carla.Horarios.Add(new HorarioOdontologo { DiaSemana = DayOfWeek.Wednesday, HoraInicio = new(8, 0), HoraFin = new(12, 0) });
        db.Odontologos.AddRange(ana, luis, carla, pedro);

        // ---------- Pacientes ----------
        var ahora = reloj.GetLocalNow().DateTime;
        var pacientes = new[]
        {
            new Paciente { Nombres = "María", Apellidos = "Gutiérrez López", CI = "1234567", FechaNacimiento = new(1990, 5, 12), Telefono = "71111111", Email = "maria.g@correo.com", Direccion = "Av. Banzer 123", FechaRegistro = ahora },
            new Paciente { Nombres = "Jorge", Apellidos = "Pérez Salvatierra", CI = "2345678", FechaNacimiento = new(1985, 9, 3), Telefono = "72222222", FechaRegistro = ahora },
            new Paciente { Nombres = "Lucía", Apellidos = "Áñez Justiniano", CI = "3456789", FechaNacimiento = new(2001, 1, 25), Telefono = "73333333", Email = "lucia.a@correo.com", FechaRegistro = ahora },
            new Paciente { Nombres = "Diego", Apellidos = "Vargas Montaño", CI = "4567890", FechaNacimiento = new(2012, 11, 30), Telefono = "74444444", Direccion = "Calle Sucre 45", FechaRegistro = ahora },
            new Paciente { Nombres = "Sofía", Apellidos = "Choque Mamani", CI = "5678901", FechaNacimiento = new(1978, 7, 8), Telefono = "75555555", FechaRegistro = ahora },
        };
        db.Pacientes.AddRange(pacientes);

        // ---------- Historial pasado (cita atendida + tratamiento + pago parcial) ----------
        var ultimoDiaHabil = AnteriorDiaHabil(hoy);
        var citaPasada = new Cita
        {
            Paciente = pacientes[0], Odontologo = ana, FechaHora = ultimoDiaHabil.ToDateTime(new TimeOnly(9, 0)),
            DuracionMinutos = 30, Motivo = "Control y limpieza", Estado = EstadoCita.Atendida, FechaRegistro = ahora.AddDays(-7)
        };
        var limpieza = new Tratamiento
        {
            Paciente = pacientes[0], Odontologo = ana, Cita = citaPasada, Tipo = TipoTratamiento.LimpiezaDental,
            Fecha = ultimoDiaHabil, Costo = 250m, Observaciones = "Sarro moderado. Control en 6 meses."
        };
        limpieza.Pagos.Add(new Pago { Monto = 100m, Metodo = MetodoPago.Efectivo, FechaPago = ultimoDiaHabil.ToDateTime(new TimeOnly(9, 40)), NumeroComprobante = "CMP-000001" });
        db.Citas.Add(citaPasada);
        db.Tratamientos.Add(limpieza);

        // ---------- Citas futuras (próximos días hábiles) ----------
        var dia1 = SiguienteDiaHabil(hoy);
        var dia2 = SiguienteDiaHabil(dia1);
        db.Citas.AddRange(
            new Cita { Paciente = pacientes[1], Odontologo = ana, FechaHora = dia1.ToDateTime(new TimeOnly(9, 0)), DuracionMinutos = 30, Motivo = "Dolor de muela", FechaRegistro = ahora },
            new Cita { Paciente = pacientes[2], Odontologo = ana, FechaHora = dia1.ToDateTime(new TimeOnly(10, 0)), DuracionMinutos = 60, Motivo = "Evaluación general", FechaRegistro = ahora },
            new Cita { Paciente = pacientes[3], Odontologo = luis, FechaHora = dia1.ToDateTime(new TimeOnly(15, 0)), DuracionMinutos = 45, Motivo = "Control de brackets", FechaRegistro = ahora },
            new Cita { Paciente = pacientes[4], Odontologo = ana, FechaHora = dia2.ToDateTime(new TimeOnly(8, 30)), DuracionMinutos = 30, Motivo = "Limpieza", FechaRegistro = ahora },
            new Cita { Paciente = pacientes[0], Odontologo = luis, FechaHora = dia2.ToDateTime(new TimeOnly(16, 0)), DuracionMinutos = 30, Motivo = "Consulta ortodoncia", Estado = EstadoCita.Cancelada, MotivoCancelacion = "Viaje del paciente", FechaRegistro = ahora }
        );

        db.SaveChanges();
    }

    private static DateOnly SiguienteDiaHabil(DateOnly desde)
    {
        var d = desde.AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static DateOnly AnteriorDiaHabil(DateOnly desde)
    {
        var d = desde.AddDays(-1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(-1);
        return d;
    }
}
