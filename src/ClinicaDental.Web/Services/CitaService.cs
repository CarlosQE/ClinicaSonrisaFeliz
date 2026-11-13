using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Services;

/// <summary>
/// Módulo 3 - Gestión de Citas. RESPONSABLE: AndresFundamentalistas.
/// IMPLEMENTACIÓN DE REFERENCIA: los demás grupos pueden copiar su estructura
/// (validaciones -> excepciones del contrato -> guardar).
/// </summary>
public class CitaService : ICitaService
{
    public const int DuracionMinima = 15;
    public const int DuracionMaxima = 240;

    private readonly ClinicaDbContext _db;
    private readonly TimeProvider _reloj;

    public CitaService(ClinicaDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    /// <summary>Hora local actual. Se usa TimeProvider para poder "congelar" el tiempo en los tests.</summary>
    private DateTime Ahora => _reloj.GetLocalNow().DateTime;

    // ------------------------------------------------------------------ consultas

    public async Task<List<Cita>> ListarAsync(DateOnly? desde = null, DateOnly? hasta = null,
        int? odontologoId = null, int? pacienteId = null, EstadoCita? estado = null)
    {
        var query = ConsultaConRelaciones();

        if (desde.HasValue)
        {
            var inicio = desde.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(c => c.FechaHora >= inicio);
        }
        if (hasta.HasValue)
        {
            var finExclusivo = hasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(c => c.FechaHora < finExclusivo);
        }
        if (odontologoId.HasValue)
            query = query.Where(c => c.OdontologoId == odontologoId.Value);
        if (pacienteId.HasValue)
            query = query.Where(c => c.PacienteId == pacienteId.Value);
        if (estado.HasValue)
            query = query.Where(c => c.Estado == estado.Value);

        return await query.OrderBy(c => c.FechaHora).ThenBy(c => c.Id).ToListAsync();
    }

    public Task<Cita?> ObtenerPorIdAsync(int id)
        => ConsultaConRelaciones().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Cita>> ObtenerAgendaDiariaAsync(DateOnly fecha, int? odontologoId = null, bool incluirCanceladas = false)
    {
        var citas = await ListarAsync(fecha, fecha, odontologoId);
        return incluirCanceladas
            ? citas
            : citas.Where(c => c.Estado != EstadoCita.Cancelada).ToList();
    }

    public async Task<List<DateTime>> ObtenerHorariosDisponiblesAsync(int odontologoId, DateOnly fecha, int duracionMinutos = 30)
    {
        if (duracionMinutos < DuracionMinima || duracionMinutos > DuracionMaxima)
            throw new ReglaNegocioException($"La duración debe estar entre {DuracionMinima} y {DuracionMaxima} minutos.");

        var odontologo = await _db.Odontologos
            .Include(o => o.Horarios)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == odontologoId)
            ?? throw new NoEncontradoException("Odontólogo", odontologoId);

        if (!odontologo.Activo) return new List<DateTime>();

        var ocupadas = await CitasQueOcupanAsync(c => c.OdontologoId == odontologoId, fecha, excluirCitaId: null);
        var ahora = Ahora;
        var libres = new List<DateTime>();

        foreach (var franja in odontologo.Horarios
                     .Where(h => h.DiaSemana == fecha.DayOfWeek)
                     .OrderBy(h => h.HoraInicio))
        {
            var inicio = fecha.ToDateTime(franja.HoraInicio);
            var limite = fecha.ToDateTime(franja.HoraFin);

            for (var t = inicio; t.AddMinutes(duracionMinutos) <= limite; t = t.AddMinutes(duracionMinutos))
            {
                var fin = t.AddMinutes(duracionMinutos);
                if (t <= ahora) continue;
                if (ocupadas.Any(c => SeSolapan(t, fin, c.FechaHora, c.FechaHoraFin))) continue;
                libres.Add(t);
            }
        }
        return libres;
    }

    // ------------------------------------------------------------------ comandos

    public async Task<Cita> AgendarAsync(Cita cita)
    {
        // R8 y R4: validaciones que no necesitan base de datos.
        if (string.IsNullOrWhiteSpace(cita.Motivo))
            throw new ReglaNegocioException("El motivo de la cita es obligatorio.");
        ValidarDuracion(cita.DuracionMinutos);

        // R1
        if (!await _db.Pacientes.AnyAsync(p => p.Id == cita.PacienteId))
            throw new ReglaNegocioException("El paciente seleccionado no existe.");

        // R2, R3, R5, R6, R7
        await ValidarDisponibilidadAsync(cita.PacienteId, cita.OdontologoId, cita.FechaHora, cita.DuracionMinutos, excluirCitaId: null);

        var nueva = new Cita
        {
            PacienteId = cita.PacienteId,
            OdontologoId = cita.OdontologoId,
            FechaHora = cita.FechaHora,
            DuracionMinutos = cita.DuracionMinutos,
            Motivo = cita.Motivo.Trim(),
            Estado = EstadoCita.Programada,
            FechaRegistro = Ahora
        };

        _db.Citas.Add(nueva);
        await _db.SaveChangesAsync();
        cita.Id = nueva.Id;
        return nueva;
    }

    public async Task<Cita> ReprogramarAsync(int citaId, DateTime nuevaFechaHora, int? nuevoOdontologoId = null)
    {
        var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == citaId)
            ?? throw new NoEncontradoException("Cita", citaId);

        if (!cita.EstaVigente)
            throw new ReglaNegocioException($"No se puede reprogramar una cita en estado {cita.Estado}.");

        var odontologoId = nuevoOdontologoId ?? cita.OdontologoId;
        await ValidarDisponibilidadAsync(cita.PacienteId, odontologoId, nuevaFechaHora, cita.DuracionMinutos, excluirCitaId: cita.Id);

        cita.FechaHora = nuevaFechaHora;
        cita.OdontologoId = odontologoId;
        cita.Estado = EstadoCita.Reprogramada;
        await _db.SaveChangesAsync();
        return cita;
    }

    public async Task CancelarAsync(int citaId, string motivo)
    {
        var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == citaId)
            ?? throw new NoEncontradoException("Cita", citaId);

        if (!cita.EstaVigente)
            throw new ReglaNegocioException($"No se puede cancelar una cita en estado {cita.Estado}.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ReglaNegocioException("Debe indicar el motivo de la cancelación.");

        cita.Estado = EstadoCita.Cancelada;
        cita.MotivoCancelacion = motivo.Trim();
        await _db.SaveChangesAsync();
    }

    public async Task MarcarAtendidaAsync(int citaId)
    {
        var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == citaId)
            ?? throw new NoEncontradoException("Cita", citaId);

        if (!cita.EstaVigente)
            throw new ReglaNegocioException($"No se puede marcar como atendida una cita en estado {cita.Estado}.");
        if (cita.FechaHora > Ahora)
            throw new ReglaNegocioException("No se puede marcar como atendida una cita que aún no comenzó.");

        cita.Estado = EstadoCita.Atendida;
        await _db.SaveChangesAsync();
    }

    // ------------------------------------------------------------------ reglas

    private static void ValidarDuracion(int minutos)
    {
        if (minutos < DuracionMinima || minutos > DuracionMaxima)
            throw new ReglaNegocioException($"La duración debe estar entre {DuracionMinima} y {DuracionMaxima} minutos.");
    }

    private async Task ValidarDisponibilidadAsync(int pacienteId, int odontologoId, DateTime inicio, int duracion, int? excluirCitaId)
    {
        var fin = inicio.AddMinutes(duracion);

        // R2
        var odontologo = await _db.Odontologos
            .Include(o => o.Horarios)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == odontologoId)
            ?? throw new ReglaNegocioException("El odontólogo seleccionado no existe.");
        if (!odontologo.Activo)
            throw new ReglaNegocioException($"{odontologo.NombreCompleto} está inactivo y no puede recibir citas.");

        // R3
        if (inicio <= Ahora)
            throw new ReglaNegocioException("La cita debe programarse para una fecha y hora futura.");

        // R5
        var dentroDeHorario = fin.Date == inicio.Date && odontologo.Horarios.Any(h =>
            h.DiaSemana == inicio.DayOfWeek &&
            h.HoraInicio <= TimeOnly.FromDateTime(inicio) &&
            TimeOnly.FromDateTime(fin) <= h.HoraFin);
        if (!dentroDeHorario)
            throw new ReglaNegocioException($"El horario elegido está fuera del horario de trabajo de {odontologo.NombreCompleto}.");

        var dia = DateOnly.FromDateTime(inicio);

        // R6
        var delOdontologo = await CitasQueOcupanAsync(c => c.OdontologoId == odontologoId, dia, excluirCitaId);
        if (delOdontologo.Any(c => SeSolapan(inicio, fin, c.FechaHora, c.FechaHoraFin)))
            throw new ReglaNegocioException($"{odontologo.NombreCompleto} ya tiene una cita en ese horario.");

        // R7
        var delPaciente = await CitasQueOcupanAsync(c => c.PacienteId == pacienteId, dia, excluirCitaId);
        if (delPaciente.Any(c => SeSolapan(inicio, fin, c.FechaHora, c.FechaHoraFin)))
            throw new ReglaNegocioException("El paciente ya tiene otra cita en ese horario.");
    }

    /// <summary>Citas NO canceladas de un día que cumplen el filtro (para revisar solapamientos en memoria).</summary>
    private async Task<List<Cita>> CitasQueOcupanAsync(
        System.Linq.Expressions.Expression<Func<Cita, bool>> filtro, DateOnly dia, int? excluirCitaId)
    {
        var desde = dia.ToDateTime(TimeOnly.MinValue);
        var hasta = desde.AddDays(1);

        var query = _db.Citas.AsNoTracking()
            .Where(filtro)
            .Where(c => c.FechaHora >= desde && c.FechaHora < hasta && c.Estado != EstadoCita.Cancelada);

        if (excluirCitaId.HasValue)
            query = query.Where(c => c.Id != excluirCitaId.Value);

        return await query.ToListAsync();
    }

    private static bool SeSolapan(DateTime inicioA, DateTime finA, DateTime inicioB, DateTime finB)
        => inicioA < finB && inicioB < finA;

    private IQueryable<Cita> ConsultaConRelaciones()
        => _db.Citas.AsNoTracking()
            .Include(c => c.Paciente)
            .Include(c => c.Odontologo);
}
