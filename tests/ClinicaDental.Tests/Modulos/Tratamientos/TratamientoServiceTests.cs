using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Common;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Modulos.Tratamientos;

/// <summary>
/// Módulo 4 - Gestión de Tratamientos (Ultimo).
/// Estas pruebas definen el comportamiento esperado de ITratamientoService.
/// Al inicio TODAS fallan (NotImplementedException). El módulo está terminado cuando todas pasan:
///     dotnet test --filter "Modulo=Tratamientos"
/// NO modifiques estas pruebas para que pasen: modifica TratamientoService.cs.
/// </summary>
[Trait("Modulo", Modulo.Tratamientos)]
public class TratamientoServiceTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;
    private readonly TratamientoService _servicio;

    public TratamientoServiceTests()
    {
        _e = _bd.Escenario();
        var db = _bd.NuevoContexto();
        _servicio = new TratamientoService(db, new CitaService(db, _bd.Reloj));
    }

    public void Dispose() => _bd.Dispose();

    private Tratamiento Nuevo(int pacienteId, int odontologoId, TipoTratamiento tipo = TipoTratamiento.Ortodoncia,
        decimal costo = 3500m, int? citaId = null)
        => new()
        {
            PacienteId = pacienteId,
            OdontologoId = odontologoId,
            CitaId = citaId,
            Tipo = tipo,
            Fecha = DateOnly.FromDateTime(_bd.Reloj.Ahora),
            Costo = costo,
            Observaciones = "Inicio de tratamiento"
        };

    // ------------------------------------------------------------------ Registrar

    [Theory]
    [InlineData(TipoTratamiento.LimpiezaDental)]
    [InlineData(TipoTratamiento.Ortodoncia)]
    [InlineData(TipoTratamiento.Endodoncia)]
    [InlineData(TipoTratamiento.Extraccion)]
    public async Task Registrar_CadaTipo_SinCita_SeGuarda(TipoTratamiento tipo)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();

        var t = await _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, tipo));

        Assert.True(t.Id > 0);
        using var db = _bd.NuevoContexto();
        var guardado = await db.Tratamientos.SingleAsync();
        Assert.Equal(tipo, guardado.Tipo);
        Assert.Equal(3500m, guardado.Costo);
        Assert.Equal("Inicio de tratamiento", guardado.Observaciones);
    }

    [Fact]
    public async Task Registrar_ConCitaVigenteDelPaciente_MarcaLaCitaComoAtendida()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _bd.Reloj.Ahora.AddMinutes(-30));

        await _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, citaId: cita.Id));

        using var db = _bd.NuevoContexto();
        Assert.Equal(EstadoCita.Atendida, (await db.Citas.SingleAsync()).Estado);
        Assert.Equal(cita.Id, (await db.Tratamientos.SingleAsync()).CitaId);
    }

    [Fact]
    public async Task Registrar_ConCitaYaAtendida_EsValido()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _bd.Reloj.Ahora.AddHours(-1), estado: EstadoCita.Atendida);

        var t = await _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, citaId: cita.Id));
        Assert.True(t.Id > 0);
    }

    [Fact]
    public async Task Registrar_ConCitaDeOtroPaciente_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var otro = await _e.PacienteAsync("Otro", "Paciente");
        var cita = await _e.CitaAsync(otro.Id, o.Id, _bd.Reloj.Ahora.AddMinutes(-30));

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, citaId: cita.Id)));
    }

    [Fact]
    public async Task Registrar_ConCitaCancelada_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _bd.Reloj.Ahora.AddMinutes(-30), estado: EstadoCita.Cancelada);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, citaId: cita.Id)));
    }

    [Fact]
    public async Task Registrar_ConCitaInexistente_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, citaId: 999)));
    }

    [Fact]
    public async Task Registrar_PacienteUOdontologoInexistente_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(999, o.Id)));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(p.Id, 999)));
    }

    [Fact]
    public async Task Registrar_CostoNegativo_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(p.Id, o.Id, costo: -1m)));
    }

    // ------------------------------------------------------------------ Listar / Obtener

    [Fact]
    public async Task Listar_DelMasRecienteAlMasAntiguo_ConFiltros()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        await _e.TratamientoAsync(p1.Id, o.Id, tipo: TipoTratamiento.LimpiezaDental, fecha: new DateOnly(2029, 1, 10));
        await _e.TratamientoAsync(p1.Id, o.Id, tipo: TipoTratamiento.Extraccion, fecha: new DateOnly(2029, 6, 10));
        await _e.TratamientoAsync(p2.Id, o.Id, tipo: TipoTratamiento.LimpiezaDental, fecha: new DateOnly(2029, 3, 10));

        var todos = await _servicio.ListarAsync();
        var delP1 = await _servicio.ListarAsync(pacienteId: p1.Id);
        var limpiezas = await _servicio.ListarAsync(tipo: TipoTratamiento.LimpiezaDental);

        Assert.Equal(new[] { 6, 3, 1 }, todos.Select(t => t.Fecha.Month));
        Assert.All(todos, t => { Assert.NotNull(t.Paciente); Assert.NotNull(t.Odontologo); });
        Assert.Equal(2, delP1.Count);
        Assert.Equal(2, limpiezas.Count);
    }

    [Fact]
    public async Task ObtenerPorId_IncluyeRelacionesYPagos_ONull()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id, costo: 500m);
        await _e.PagoAsync(t.Id, 100m);

        var r = await _servicio.ObtenerPorIdAsync(t.Id);

        Assert.NotNull(r!.Paciente);
        Assert.NotNull(r.Odontologo);
        Assert.Single(r.Pagos);
        Assert.Null(await _servicio.ObtenerPorIdAsync(999));
    }

    // ------------------------------------------------------------------ Observaciones

    [Fact]
    public async Task ActualizarObservaciones_ReemplazaElTexto()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id);

        await _servicio.ActualizarObservacionesAsync(t.Id, "Paciente con sensibilidad en pieza 36.");

        using var db = _bd.NuevoContexto();
        Assert.Equal("Paciente con sensibilidad en pieza 36.", (await db.Tratamientos.SingleAsync()).Observaciones);
    }

    [Fact]
    public async Task ActualizarObservaciones_Inexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ActualizarObservacionesAsync(999, "x"));
    }

    // ------------------------------------------------------------------ Eliminar

    [Fact]
    public async Task Eliminar_SinPagos_LoBorra()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id);

        await _servicio.EliminarAsync(t.Id);

        using var db = _bd.NuevoContexto();
        Assert.Empty(db.Tratamientos);
    }

    [Fact]
    public async Task Eliminar_ConPagos_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id);
        await _e.PagoAsync(t.Id, 50m);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.EliminarAsync(t.Id));
    }

    [Fact]
    public async Task Eliminar_Inexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.EliminarAsync(999));
    }
}
