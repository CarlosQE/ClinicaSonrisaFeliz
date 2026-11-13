using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Services.Contratos;

/// <summary>
/// CONTRATO del módulo 3 - Gestión de Citas (grupo AndresFundamentalistas).
/// Pruebas que lo validan: tests/ClinicaDental.Tests/Modulos/Citas
///
/// Reglas comunes al agendar y reprogramar (todas -> ReglaNegocioException):
///   R1. El paciente debe existir.
///   R2. El odontólogo debe existir y estar Activo.
///   R3. La fecha/hora debe ser futura.
///   R4. La duración debe estar entre 15 y 240 minutos.
///   R5. La cita debe caer COMPLETA dentro de una franja de horario del odontólogo ese día de la semana.
///   R6. El odontólogo no puede tener otra cita no cancelada que se solape.
///   R7. El paciente no puede tener otra cita no cancelada que se solape.
///   R8. El motivo es obligatorio.
/// </summary>
public interface ICitaService
{
    /// <summary>
    /// Lista citas (con Paciente y Odontologo) ordenadas por FechaHora ascendente, aplicando los filtros no nulos.
    /// <paramref name="desde"/> y <paramref name="hasta"/> son inclusivos (por día).
    /// </summary>
    Task<List<Cita>> ListarAsync(DateOnly? desde = null, DateOnly? hasta = null,
        int? odontologoId = null, int? pacienteId = null, EstadoCita? estado = null);

    /// <summary>Devuelve la cita con Paciente y Odontologo, o null si no existe.</summary>
    Task<Cita?> ObtenerPorIdAsync(int id);

    /// <summary>Agenda una cita nueva (Estado = Programada, FechaRegistro = ahora). Aplica R1..R8.</summary>
    Task<Cita> AgendarAsync(Cita cita);

    /// <summary>
    /// Cambia fecha/hora (y opcionalmente odontólogo) de una cita vigente. Estado pasa a Reprogramada.
    /// No existe -> NoEncontradoException. Cancelada o Atendida -> ReglaNegocioException. Aplica R2..R7 (ignorando la propia cita).
    /// </summary>
    Task<Cita> ReprogramarAsync(int citaId, DateTime nuevaFechaHora, int? nuevoOdontologoId = null);

    /// <summary>
    /// Cancela una cita vigente guardando el motivo. Libera el horario.
    /// No existe -> NoEncontradoException. Ya Cancelada/Atendida o motivo vacío -> ReglaNegocioException.
    /// </summary>
    Task CancelarAsync(int citaId, string motivo);

    /// <summary>
    /// Marca una cita vigente como Atendida (la usa el módulo 4 al registrar tratamientos).
    /// No existe -> NoEncontradoException. No vigente o cita aún no iniciada -> ReglaNegocioException.
    /// </summary>
    Task MarcarAtendidaAsync(int citaId);

    /// <summary>
    /// Agenda diaria: citas del día (con Paciente y Odontologo) ordenadas por hora.
    /// Por defecto excluye las canceladas. Filtra por odontólogo si se indica.
    /// </summary>
    Task<List<Cita>> ObtenerAgendaDiariaAsync(DateOnly fecha, int? odontologoId = null, bool incluirCanceladas = false);

    /// <summary>
    /// Horarios libres de un odontólogo en un día, en bloques de <paramref name="duracionMinutos"/>,
    /// respetando sus franjas de trabajo y las citas no canceladas. Solo devuelve horas futuras.
    /// Odontólogo no existe -> NoEncontradoException. Odontólogo inactivo -> lista vacía.
    /// </summary>
    Task<List<DateTime>> ObtenerHorariosDisponiblesAsync(int odontologoId, DateOnly fecha, int duracionMinutos = 30);
}
