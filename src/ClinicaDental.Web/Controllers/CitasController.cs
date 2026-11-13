using ClinicaDental.Web.Common;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;
using ClinicaDental.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Controllers;

/// <summary>
/// Módulo 3 - Gestión de Citas (AndresFundamentalistas). CONTROLADOR DE REFERENCIA.
/// Patrón estándar del proyecto:
///   1. Validar ModelState.
///   2. Llamar al servicio (toda la lógica de negocio vive en el servicio, NO aquí).
///   3. ReglaNegocioException -> ModelState.AddModelError + volver a mostrar el formulario.
///   4. Éxito -> TempData["Exito"] + RedirectToAction (patrón PRG).
///   (NoEncontradoException la convierte en 404 el filtro global.)
/// </summary>
public class CitasController : Controller
{
    private readonly ICitaService _citas;
    private readonly ClinicaDbContext _db;
    private readonly TimeProvider _reloj;

    public CitasController(ICitaService citas, ClinicaDbContext db, TimeProvider reloj)
    {
        _citas = citas;
        _db = db;
        _reloj = reloj;
    }

    private DateOnly Hoy => DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);

    // GET /Citas
    public async Task<IActionResult> Index(DateOnly? desde, DateOnly? hasta, int? odontologoId, EstadoCita? estado)
    {
        desde ??= Hoy;
        var vm = new ListadoCitasViewModel
        {
            Desde = desde,
            Hasta = hasta,
            OdontologoId = odontologoId,
            Estado = estado,
            Citas = await _citas.ListarAsync(desde, hasta, odontologoId, null, estado),
            Odontologos = await OpcionesOdontologosAsync(soloActivos: false, odontologoId)
        };
        return View(vm);
    }

    // GET /Citas/Agenda?fecha=2026-11-13
    public async Task<IActionResult> Agenda(DateOnly? fecha, int? odontologoId, bool incluirCanceladas = false)
    {
        var dia = fecha ?? Hoy;
        var vm = new AgendaDiariaViewModel
        {
            Fecha = dia,
            OdontologoId = odontologoId,
            IncluirCanceladas = incluirCanceladas,
            Citas = await _citas.ObtenerAgendaDiariaAsync(dia, odontologoId, incluirCanceladas),
            Odontologos = await OpcionesOdontologosAsync(soloActivos: false, odontologoId)
        };
        return View(vm);
    }

    // GET /Citas/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var cita = await _citas.ObtenerPorIdAsync(id);
        return cita is null ? NotFound() : View(cita);
    }

    // GET /Citas/Create
    public async Task<IActionResult> Create(int? pacienteId, int? odontologoId, DateTime? fechaHora)
    {
        var vm = new AgendarCitaViewModel { PacienteId = pacienteId, OdontologoId = odontologoId, FechaHora = fechaHora };
        await CargarListasAsync(vm);
        return View(vm);
    }

    // POST /Citas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AgendarCitaViewModel vm)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var cita = await _citas.AgendarAsync(new Cita
                {
                    PacienteId = vm.PacienteId!.Value,
                    OdontologoId = vm.OdontologoId!.Value,
                    FechaHora = vm.FechaHora!.Value,
                    DuracionMinutos = vm.DuracionMinutos,
                    Motivo = vm.Motivo
                });
                TempData["Exito"] = $"Cita agendada para el {cita.FechaHora:dd/MM/yyyy HH:mm}.";
                return RedirectToAction(nameof(Agenda), new { fecha = DateOnly.FromDateTime(cita.FechaHora).ToString("yyyy-MM-dd") });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        await CargarListasAsync(vm);
        return View(vm);
    }

    // GET /Citas/Reprogramar/5
    public async Task<IActionResult> Reprogramar(int id)
    {
        var cita = await _citas.ObtenerPorIdAsync(id);
        if (cita is null) return NotFound();

        var vm = new ReprogramarCitaViewModel
        {
            Id = id,
            Cita = cita,
            NuevaFechaHora = cita.FechaHora,
            OdontologoId = cita.OdontologoId,
            Odontologos = await OpcionesOdontologosAsync(soloActivos: true, cita.OdontologoId)
        };
        return View(vm);
    }

    // POST /Citas/Reprogramar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reprogramar(int id, ReprogramarCitaViewModel vm)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var cita = await _citas.ReprogramarAsync(id, vm.NuevaFechaHora!.Value, vm.OdontologoId);
                TempData["Exito"] = $"Cita reprogramada para el {cita.FechaHora:dd/MM/yyyy HH:mm}.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        vm.Id = id;
        vm.Cita = await _citas.ObtenerPorIdAsync(id);
        if (vm.Cita is null) return NotFound();
        vm.Odontologos = await OpcionesOdontologosAsync(soloActivos: true, vm.OdontologoId);
        return View(vm);
    }

    // GET /Citas/Cancelar/5
    public async Task<IActionResult> Cancelar(int id)
    {
        var cita = await _citas.ObtenerPorIdAsync(id);
        if (cita is null) return NotFound();
        return View(new CancelarCitaViewModel { Id = id, Cita = cita });
    }

    // POST /Citas/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id, CancelarCitaViewModel vm)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _citas.CancelarAsync(id, vm.Motivo);
                TempData["Exito"] = "La cita fue cancelada.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        vm.Id = id;
        vm.Cita = await _citas.ObtenerPorIdAsync(id);
        if (vm.Cita is null) return NotFound();
        return View(vm);
    }

    // POST /Citas/MarcarAtendida/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarAtendida(int id)
    {
        try
        {
            await _citas.MarcarAtendidaAsync(id);
            TempData["Exito"] = "La cita fue marcada como atendida.";
        }
        catch (ReglaNegocioException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET /Citas/HorariosDisponibles?odontologoId=1&fecha=2026-11-13&duracion=30   (JSON para el formulario)
    [HttpGet]
    public async Task<IActionResult> HorariosDisponibles(int odontologoId, DateOnly fecha, int duracion = 30)
    {
        try
        {
            var libres = await _citas.ObtenerHorariosDisponiblesAsync(odontologoId, fecha, duracion);
            return Json(libres.Select(h => h.ToString("yyyy-MM-ddTHH:mm")));
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ------------------------------------------------------------------ listas desplegables
    // Se lee directo del DbContext (solo lectura) para NO depender de que los módulos
    // de Pacientes y Odontólogos estén terminados.

    private async Task CargarListasAsync(AgendarCitaViewModel vm)
    {
        vm.Pacientes = await _db.Pacientes.AsNoTracking()
            .OrderBy(p => p.Apellidos).ThenBy(p => p.Nombres)
            .Select(p => new SelectListItem($"{p.Apellidos}, {p.Nombres} (CI {p.CI})", p.Id.ToString(), p.Id == vm.PacienteId))
            .ToListAsync();
        vm.Odontologos = await OpcionesOdontologosAsync(soloActivos: true, vm.OdontologoId);
    }

    private async Task<List<SelectListItem>> OpcionesOdontologosAsync(bool soloActivos, int? seleccionado)
    {
        var query = _db.Odontologos.AsNoTracking().Include(o => o.Especialidad).AsQueryable();
        if (soloActivos) query = query.Where(o => o.Activo);

        var lista = await query.OrderBy(o => o.Apellidos).ThenBy(o => o.Nombres).ToListAsync();
        return lista.Select(o => new SelectListItem(
            $"{o.NombreCompleto} - {o.Especialidad?.Nombre}{(o.Activo ? "" : " (inactivo)")}",
            o.Id.ToString(), o.Id == seleccionado)).ToList();
    }
}
