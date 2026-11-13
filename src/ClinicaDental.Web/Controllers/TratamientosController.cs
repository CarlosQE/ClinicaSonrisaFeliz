using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers;

/// <summary>
/// Módulo 4 - Gestión de Tratamientos. RESPONSABLE: Ultimo.
/// TODO(Ultimo): agregar Create, Details, Observaciones y Delete
/// siguiendo el patrón de CitasController (PRG + ReglaNegocioException -> ModelState).
/// RUTAS OBLIGATORIAS (las prueban los tests de integración):
///   GET /Tratamientos                       -> listado (acepta ?pacienteId= y ?tipo=)
///   GET /Tratamientos/Create                -> formulario (acepta ?citaId= para venir desde una cita)
///   GET /Tratamientos/Details/{id}          -> detalle con pagos
///   GET /Tratamientos/Observaciones/{id}    -> registro de observaciones
/// </summary>
public class TratamientosController : Controller
{
    private readonly ITratamientoService _tratamientos;

    public TratamientosController(ITratamientoService tratamientos) => _tratamientos = tratamientos;

    // GET /Tratamientos
    public async Task<IActionResult> Index(int? pacienteId, TipoTratamiento? tipo)
        => View(await _tratamientos.ListarAsync(pacienteId, tipo));
}
