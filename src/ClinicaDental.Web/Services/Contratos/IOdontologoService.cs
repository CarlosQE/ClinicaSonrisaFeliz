using ClinicaDental.Web.Models;

namespace ClinicaDental.Web.Services.Contratos;

/// <summary>
/// CONTRATO del módulo 2 - Gestión de Odontólogos (grupo BagayerosChaco).
/// Firmas CONGELADAS: no cambiar nombres, parámetros ni tipos de retorno sin acuerdo del líder.
/// Pruebas que lo validan: tests/ClinicaDental.Tests/Modulos/Odontologos
/// </summary>
public interface IOdontologoService
{
    /// <summary>
    /// Lista odontólogos (incluyendo Especialidad) ordenados por Apellidos y Nombres.
    /// Si <paramref name="soloActivos"/> es true, excluye los inactivos.
    /// </summary>
    Task<List<Odontologo>> ListarAsync(bool soloActivos = false);

    /// <summary>Devuelve el odontólogo con Especialidad y Horarios cargados, o null si no existe.</summary>
    Task<Odontologo?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Registra un odontólogo (Activo = true). Reglas -> ReglaNegocioException:
    /// Matrícula única; EspecialidadId debe existir.
    /// </summary>
    Task<Odontologo> RegistrarAsync(Odontologo odontologo);

    /// <summary>
    /// Actualiza Nombres, Apellidos, Matricula, Telefono, Email y EspecialidadId.
    /// No existe -> NoEncontradoException. Matrícula repetida en OTRO odontólogo o especialidad inexistente -> ReglaNegocioException.
    /// </summary>
    Task ActualizarAsync(Odontologo odontologo);

    /// <summary>Activa / desactiva al odontólogo. No existe -> NoEncontradoException.</summary>
    Task CambiarEstadoAsync(int id, bool activo);

    // ---------------- Especialidades ----------------

    /// <summary>Lista especialidades ordenadas por Nombre.</summary>
    Task<List<Especialidad>> ListarEspecialidadesAsync();

    /// <summary>Registra una especialidad. Nombre repetido (sin distinguir mayúsculas) -> ReglaNegocioException.</summary>
    Task<Especialidad> RegistrarEspecialidadAsync(Especialidad especialidad);

    // ---------------- Horarios de trabajo ----------------

    /// <summary>Horarios del odontólogo ordenados por DiaSemana y HoraInicio.</summary>
    Task<List<HorarioOdontologo>> ListarHorariosAsync(int odontologoId);

    /// <summary>
    /// Agrega una franja de trabajo. Odontólogo no existe -> NoEncontradoException.
    /// Reglas -> ReglaNegocioException: HoraFin debe ser mayor que HoraInicio;
    /// no puede solaparse con otra franja del mismo odontólogo el mismo día.
    /// </summary>
    Task<HorarioOdontologo> AgregarHorarioAsync(HorarioOdontologo horario);

    /// <summary>Elimina una franja de trabajo. No existe -> NoEncontradoException.</summary>
    Task EliminarHorarioAsync(int horarioId);
}
