using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Services.Contratos;

/// <summary>
/// CONTRATO del módulo 4 - Gestión de Tratamientos (grupo Ultimo).
/// Firmas CONGELADAS: no cambiar nombres, parámetros ni tipos de retorno sin acuerdo del líder.
/// Pruebas que lo validan: tests/ClinicaDental.Tests/Modulos/Tratamientos
/// </summary>
public interface ITratamientoService
{
    /// <summary>
    /// Lista tratamientos (con Paciente y Odontologo) del más reciente al más antiguo (por Fecha, luego Id),
    /// filtrando por paciente y/o tipo si se indican.
    /// </summary>
    Task<List<Tratamiento>> ListarAsync(int? pacienteId = null, TipoTratamiento? tipo = null);

    /// <summary>Devuelve el tratamiento con Paciente, Odontologo, Cita y Pagos, o null si no existe.</summary>
    Task<Tratamiento?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Registra un tratamiento. Reglas -> ReglaNegocioException:
    /// paciente y odontólogo deben existir; Costo &gt;= 0;
    /// si trae CitaId: la cita debe existir, ser del MISMO paciente y no estar Cancelada.
    /// Si la cita estaba vigente, se marca como Atendida (usar ICitaService.MarcarAtendidaAsync o equivalente).
    /// </summary>
    Task<Tratamiento> RegistrarAsync(Tratamiento tratamiento);

    /// <summary>
    /// Registro de observaciones: reemplaza el texto de Observaciones.
    /// No existe -> NoEncontradoException.
    /// </summary>
    Task ActualizarObservacionesAsync(int tratamientoId, string? observaciones);

    /// <summary>
    /// Elimina un tratamiento. No existe -> NoEncontradoException. Tiene pagos -> ReglaNegocioException.
    /// </summary>
    Task EliminarAsync(int id);
}
