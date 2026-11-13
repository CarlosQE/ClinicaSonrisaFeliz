using ClinicaDental.Web.Models;
using ClinicaDental.Web.Models.Consultas;

namespace ClinicaDental.Web.Services.Contratos;

/// <summary>
/// CONTRATO del módulo 1 - Gestión de Pacientes (grupo ApostolesEvoMorales).
/// Firmas CONGELADAS: no cambiar nombres, parámetros ni tipos de retorno sin acuerdo del líder.
/// Pruebas que lo validan: tests/ClinicaDental.Tests/Modulos/Pacientes
/// </summary>
public interface IPacienteService
{
    /// <summary>
    /// Lista pacientes ordenados por Apellidos y luego Nombres.
    /// Si <paramref name="busqueda"/> tiene texto, filtra (sin distinguir mayúsculas) por Nombres, Apellidos o CI que lo contengan.
    /// </summary>
    Task<List<Paciente>> ListarAsync(string? busqueda = null);

    /// <summary>Devuelve el paciente o null si no existe.</summary>
    Task<Paciente?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Registra un paciente nuevo, asigna FechaRegistro = ahora y devuelve la entidad con su Id.
    /// Reglas: CI único (si ya existe -> ReglaNegocioException); FechaNacimiento no puede ser futura (ReglaNegocioException).
    /// </summary>
    Task<Paciente> RegistrarAsync(Paciente paciente);

    /// <summary>
    /// Actualiza los datos personales (Nombres, Apellidos, CI, FechaNacimiento, Telefono, Email, Direccion).
    /// No existe -> NoEncontradoException. CI repetido en OTRO paciente -> ReglaNegocioException.
    /// </summary>
    Task ActualizarAsync(Paciente paciente);

    /// <summary>
    /// Elimina un paciente. No existe -> NoEncontradoException.
    /// Si tiene citas o tratamientos registrados -> ReglaNegocioException (no se borra historial clínico).
    /// </summary>
    Task EliminarAsync(int id);

    /// <summary>
    /// Historial de atención: el paciente + todas sus citas (con Odontologo) y tratamientos (con Odontologo),
    /// ordenados del más reciente al más antiguo. No existe -> NoEncontradoException.
    /// </summary>
    Task<HistorialPaciente> ObtenerHistorialAsync(int pacienteId);
}
