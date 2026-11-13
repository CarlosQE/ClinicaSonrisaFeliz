using ClinicaDental.Web.Services.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers;

/// <summary>
/// Módulo 5 - Gestión de Pagos. RESPONSABLE: GuaposMisteriosos.
/// TODO(GuaposMisteriosos): agregar Create, Deudas y Comprobante
/// siguiendo el patrón de CitasController (PRG + ReglaNegocioException -> ModelState).
/// RUTAS OBLIGATORIAS (las prueban los tests de integración):
///   GET /Pagos                       -> listado (acepta ?pacienteId=)
///   GET /Pagos/Create                -> formulario (acepta ?tratamientoId=)
///   GET /Pagos/Deudas                -> deudas pendientes (acepta ?pacienteId=)
///   GET /Pagos/Comprobante/{id}      -> comprobante imprimible
/// </summary>
public class PagosController : Controller
{
    private readonly IPagoService _pagos;

    public PagosController(IPagoService pagos) => _pagos = pagos;

    // GET /Pagos
    public async Task<IActionResult> Index(int? pacienteId)
        => View(await _pagos.ListarAsync(pacienteId));
}
