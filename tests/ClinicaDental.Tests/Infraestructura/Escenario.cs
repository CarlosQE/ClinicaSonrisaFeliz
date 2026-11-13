using ClinicaDental.Web.Models;

namespace ClinicaDental.Tests.Infraestructura;

/// <summary>
/// Crea datos de prueba DIRECTAMENTE en la base (sin pasar por los servicios),
/// para que el test de un módulo no dependa de que otro módulo esté terminado.
///
/// Horario estándar de los odontólogos creados aquí: lunes a viernes 08:00-12:00 y 14:00-18:00.
/// </summary>
public sealed class Escenario
{
    private readonly BaseDatosPrueba _bd;
    private static int _contador;
    private static int Siguiente() => Interlocked.Increment(ref _contador);

    public Escenario(BaseDatosPrueba bd) => _bd = bd;

    public DateTime Ahora => _bd.Reloj.Ahora;

    /// <summary>Lunes siguiente a "ahora" (siempre en el futuro, día laboral).</summary>
    public DateOnly ProximoLunes
    {
        get
        {
            var d = DateOnly.FromDateTime(Ahora).AddDays(1);
            while (d.DayOfWeek != DayOfWeek.Monday) d = d.AddDays(1);
            return d;
        }
    }

    public DateTime ProximoLunesA(int hora, int minuto = 0) => ProximoLunes.ToDateTime(new TimeOnly(hora, minuto));

    public async Task<Especialidad> EspecialidadAsync(string? nombre = null)
    {
        using var db = _bd.NuevoContexto();
        var e = new Especialidad { Nombre = nombre ?? $"Especialidad {Siguiente()}" };
        db.Especialidades.Add(e);
        await db.SaveChangesAsync();
        return e;
    }

    public async Task<Paciente> PacienteAsync(string nombres = "Juan", string apellidos = "Pérez", string? ci = null)
    {
        using var db = _bd.NuevoContexto();
        var p = new Paciente
        {
            Nombres = nombres,
            Apellidos = apellidos,
            CI = ci ?? $"CI-{Siguiente():0000}",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            Telefono = "70000000",
            FechaRegistro = Ahora
        };
        db.Pacientes.Add(p);
        await db.SaveChangesAsync();
        return p;
    }

    public async Task<Odontologo> OdontologoAsync(bool activo = true, bool conHorario = true,
        string nombres = "Ana", string apellidos = "Rojas", string? matricula = null, int? especialidadId = null)
    {
        especialidadId ??= (await EspecialidadAsync()).Id;

        using var db = _bd.NuevoContexto();
        var o = new Odontologo
        {
            Nombres = nombres,
            Apellidos = apellidos,
            Matricula = matricula ?? $"MAT-{Siguiente():0000}",
            Telefono = "70000001",
            Activo = activo,
            EspecialidadId = especialidadId.Value
        };
        if (conHorario)
        {
            foreach (var dia in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            {
                o.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(8, 0), HoraFin = new(12, 0) });
                o.Horarios.Add(new HorarioOdontologo { DiaSemana = dia, HoraInicio = new(14, 0), HoraFin = new(18, 0) });
            }
        }
        db.Odontologos.Add(o);
        await db.SaveChangesAsync();
        return o;
    }

    /// <summary>Inserta una cita SIN validar reglas (útil para preparar estados como pasadas, canceladas, etc.).</summary>
    public async Task<Cita> CitaAsync(int pacienteId, int odontologoId, DateTime fechaHora,
        int duracion = 30, EstadoCita estado = EstadoCita.Programada, string motivo = "Control")
    {
        using var db = _bd.NuevoContexto();
        var c = new Cita
        {
            PacienteId = pacienteId,
            OdontologoId = odontologoId,
            FechaHora = fechaHora,
            DuracionMinutos = duracion,
            Motivo = motivo,
            Estado = estado,
            MotivoCancelacion = estado == EstadoCita.Cancelada ? "Preparado en test" : null,
            FechaRegistro = Ahora
        };
        db.Citas.Add(c);
        await db.SaveChangesAsync();
        return c;
    }

    public async Task<Tratamiento> TratamientoAsync(int pacienteId, int odontologoId, decimal costo = 500m,
        TipoTratamiento tipo = TipoTratamiento.LimpiezaDental, DateOnly? fecha = null, int? citaId = null)
    {
        using var db = _bd.NuevoContexto();
        var t = new Tratamiento
        {
            PacienteId = pacienteId,
            OdontologoId = odontologoId,
            CitaId = citaId,
            Tipo = tipo,
            Fecha = fecha ?? DateOnly.FromDateTime(Ahora),
            Costo = costo
        };
        db.Tratamientos.Add(t);
        await db.SaveChangesAsync();
        return t;
    }

    public async Task<Pago> PagoAsync(int tratamientoId, decimal monto, string? comprobante = null, DateTime? fecha = null)
    {
        using var db = _bd.NuevoContexto();
        var p = new Pago
        {
            TratamientoId = tratamientoId,
            Monto = monto,
            Metodo = MetodoPago.Efectivo,
            FechaPago = fecha ?? Ahora,
            NumeroComprobante = comprobante ?? $"PRE-{Siguiente():0000}"
        };
        db.Pagos.Add(p);
        await db.SaveChangesAsync();
        return p;
    }

    /// <summary>Paciente + odontólogo activo con horario estándar. Lo más común en los tests.</summary>
    public async Task<(Paciente paciente, Odontologo odontologo)> PacienteYOdontologoAsync()
        => (await PacienteAsync(), await OdontologoAsync());
}
