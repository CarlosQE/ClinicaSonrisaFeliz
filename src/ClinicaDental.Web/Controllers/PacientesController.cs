using ClinicaDental.Web.Services.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers;

/// <summary>
/// Módulo 1 - Gestión de Pacientes. RESPONSABLE: ApostolesEvoMorales.
/// TODO(ApostolesEvoMorales): agregar acciones Create, Edit, Delete y Historial
/// siguiendo el patrón de CitasController (PRG + ReglaNegocioException -> ModelState).
/// RUTAS OBLIGATORIAS (las prueban los tests de integración):
///   GET /Pacientes                 -> listado (acepta ?busqueda=)
///   GET /Pacientes/Create          -> formulario de registro
///   GET /Pacientes/Edit/{id}       -> formulario de edición
///   GET /Pacientes/Historial/{id}  -> historial de atención
/// </summary>
public class PacientesController : Controller
{
    private readonly IPacienteService _pacientes;

    public PacientesController(IPacienteService pacientes) => _pacientes = pacientes;

    // GET /Pacientes?busqueda=
    public async Task<IActionResult> Index(string? busqueda)
    {
        ViewData["Busqueda"] = busqueda;
        return View(await _pacientes.ListarAsync(busqueda));
    }
}
