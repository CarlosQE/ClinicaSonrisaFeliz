using ClinicaDental.Web.Services.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaDental.Web.Controllers;

/// <summary>
/// Módulo 2 - Gestión de Odontólogos. RESPONSABLE: BagayerosChaco.
/// TODO(BagayerosChaco): agregar Create, Edit, CambiarEstado, Especialidades y Horarios
/// siguiendo el patrón de CitasController (PRG + ReglaNegocioException -> ModelState).
/// RUTAS OBLIGATORIAS (las prueban los tests de integración):
///   GET /Odontologos                    -> listado
///   GET /Odontologos/Create             -> formulario de registro
///   GET /Odontologos/Edit/{id}          -> formulario de edición
///   GET /Odontologos/Horarios/{id}      -> horarios de trabajo del odontólogo
///   GET /Odontologos/Especialidades     -> listado / alta de especialidades
/// </summary>
public class OdontologosController : Controller
{
    private readonly IOdontologoService _odontologos;

    public OdontologosController(IOdontologoService odontologos) => _odontologos = odontologos;

    // GET /Odontologos
    public async Task<IActionResult> Index()
        => View(await _odontologos.ListarAsync());
}
