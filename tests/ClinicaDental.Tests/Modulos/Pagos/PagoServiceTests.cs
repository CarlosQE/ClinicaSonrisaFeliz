using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Common;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Modulos.Pagos;

/// <summary>
/// Módulo 5 - Gestión de Pagos (GuaposMisteriosos).
/// Estas pruebas definen el comportamiento esperado de IPagoService.
/// Al inicio TODAS fallan (NotImplementedException). El módulo está terminado cuando todas pasan:
///     dotnet test --filter "Modulo=Pagos"
/// NO modifiques estas pruebas para que pasen: modifica PagoService.cs.
/// </summary>
[Trait("Modulo", Modulo.Pagos)]
public class PagoServiceTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;
    private readonly PagoService _servicio;

    public PagoServiceTests()
    {
        _e = _bd.Escenario();
        _servicio = new PagoService(_bd.NuevoContexto(), _bd.Reloj);
    }

    public void Dispose() => _bd.Dispose();

    private async Task<Tratamiento> TratamientoDeAsync(decimal costo, Paciente? paciente = null)
    {
        var p = paciente ?? await _e.PacienteAsync();
        var o = await _e.OdontologoAsync();
        return await _e.TratamientoAsync(p.Id, o.Id, costo);
    }

    private static Pago Nuevo(int tratamientoId, decimal monto, MetodoPago metodo = MetodoPago.Efectivo)
        => new() { TratamientoId = tratamientoId, Monto = monto, Metodo = metodo };

    // ------------------------------------------------------------------ Registrar

    [Fact]
    public async Task Registrar_PrimerPago_AsignaComprobanteYFecha()
    {
        var t = await TratamientoDeAsync(500m);

        var pago = await _servicio.RegistrarAsync(Nuevo(t.Id, 200m, MetodoPago.QR));

        Assert.True(pago.Id > 0);
        Assert.Equal("CMP-000001", pago.NumeroComprobante);
        using var db = _bd.NuevoContexto();
        var guardado = await db.Pagos.SingleAsync();
        Assert.Equal(200m, guardado.Monto);
        Assert.Equal(MetodoPago.QR, guardado.Metodo);
        Assert.Equal(_bd.Reloj.Ahora, guardado.FechaPago);
    }

    [Fact]
    public async Task Registrar_ComprobanteEsCorrelativoAlMayorExistente()
    {
        var t = await TratamientoDeAsync(1000m);
        await _e.PagoAsync(t.Id, 10m, comprobante: "CMP-000007");

        var p1 = await _servicio.RegistrarAsync(Nuevo(t.Id, 10m));
        var p2 = await _servicio.RegistrarAsync(Nuevo(t.Id, 10m));

        Assert.Equal("CMP-000008", p1.NumeroComprobante);
        Assert.Equal("CMP-000009", p2.NumeroComprobante);
    }

    [Fact]
    public async Task Registrar_PagoExactoDelSaldo_EsValido()
    {
        var t = await TratamientoDeAsync(300m);
        await _e.PagoAsync(t.Id, 100m);

        await _servicio.RegistrarAsync(Nuevo(t.Id, 200m));

        Assert.Equal(0m, await _servicio.ObtenerSaldoPendienteAsync(t.Id));
    }

    [Fact]
    public async Task Registrar_MontoMayorAlSaldo_LanzaReglaNegocio()
    {
        var t = await TratamientoDeAsync(300m);
        await _e.PagoAsync(t.Id, 100m);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(t.Id, 200.01m)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task Registrar_MontoNoPositivo_LanzaReglaNegocio(decimal monto)
    {
        var t = await TratamientoDeAsync(300m);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(t.Id, monto)));
    }

    [Fact]
    public async Task Registrar_TratamientoInexistente_LanzaReglaNegocio()
    {
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(999, 10m)));
    }

    // ------------------------------------------------------------------ Saldo y deudas

    [Fact]
    public async Task SaldoPendiente_EsCostoMenosPagos()
    {
        var t = await TratamientoDeAsync(750.50m);
        await _e.PagoAsync(t.Id, 100m);
        await _e.PagoAsync(t.Id, 150.25m);

        Assert.Equal(500.25m, await _servicio.ObtenerSaldoPendienteAsync(t.Id));
    }

    [Fact]
    public async Task SaldoPendiente_TratamientoInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ObtenerSaldoPendienteAsync(999));
    }

    [Fact]
    public async Task ConsultarDeudas_SoloConSaldo_OrdenadasPorPaciente()
    {
        var zoe = await _e.PacienteAsync("Zoe", "Vaca");
        var ana = await _e.PacienteAsync("Ana", "Lara");
        var tz = await TratamientoDeAsync(400m, zoe);
        var ta = await TratamientoDeAsync(200m, ana);
        var pagado = await TratamientoDeAsync(100m, ana);
        await _e.PagoAsync(tz.Id, 150m);
        await _e.PagoAsync(pagado.Id, 100m);

        var deudas = await _servicio.ConsultarDeudasAsync();

        Assert.Equal(new[] { ta.Id, tz.Id }, deudas.Select(d => d.TratamientoId));
        var dz = deudas.Single(d => d.TratamientoId == tz.Id);
        Assert.Equal(zoe.Id, dz.PacienteId);
        Assert.Contains("Zoe", dz.PacienteNombre);
        Assert.Equal(400m, dz.Costo);
        Assert.Equal(150m, dz.TotalPagado);
        Assert.Equal(250m, dz.Saldo);
    }

    [Fact]
    public async Task ConsultarDeudas_FiltraPorPaciente()
    {
        var p1 = await _e.PacienteAsync();
        var p2 = await _e.PacienteAsync();
        await TratamientoDeAsync(100m, p1);
        await TratamientoDeAsync(100m, p2);

        var deudas = await _servicio.ConsultarDeudasAsync(p2.Id);

        Assert.Single(deudas);
        Assert.Equal(p2.Id, deudas[0].PacienteId);
    }

    // ------------------------------------------------------------------ Comprobante

    [Fact]
    public async Task Comprobante_ContieneDatosYSaldoDespuesDeEsePago()
    {
        var paciente = await _e.PacienteAsync("Rosa", "Quispe", ci: "7654321");
        var t = await TratamientoDeAsync(1000m, paciente);
        var p1 = await _servicio.RegistrarAsync(Nuevo(t.Id, 300m, MetodoPago.Tarjeta));
        _bd.Reloj.Ahora = _bd.Reloj.Ahora.AddDays(1);
        await _servicio.RegistrarAsync(Nuevo(t.Id, 200m));

        var c = await _servicio.ObtenerComprobanteAsync(p1.Id);

        Assert.Equal(p1.Id, c.PagoId);
        Assert.Equal("CMP-000001", c.NumeroComprobante);
        Assert.Equal("7654321", c.PacienteCI);
        Assert.Contains("Rosa", c.PacienteNombre);
        Assert.Equal(1000m, c.CostoTratamiento);
        Assert.Equal(300m, c.Monto);
        Assert.Equal(MetodoPago.Tarjeta, c.Metodo);
        Assert.Equal(700m, c.SaldoRestante); // saldo justo después del primer pago, no el actual (500)
    }

    [Fact]
    public async Task Comprobante_PagoInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ObtenerComprobanteAsync(999));
    }

    // ------------------------------------------------------------------ Listar / Obtener

    [Fact]
    public async Task Listar_DelMasRecienteAlMasAntiguo_IncluyePaciente_YFiltra()
    {
        var p1 = await _e.PacienteAsync();
        var p2 = await _e.PacienteAsync();
        var t1 = await TratamientoDeAsync(1000m, p1);
        var t2 = await TratamientoDeAsync(1000m, p2);
        var ahora = _bd.Reloj.Ahora;
        await _e.PagoAsync(t1.Id, 10m, "A", ahora.AddDays(-2));
        await _e.PagoAsync(t2.Id, 10m, "B", ahora);
        await _e.PagoAsync(t1.Id, 10m, "C", ahora.AddDays(-1));

        var todos = await _servicio.ListarAsync();
        var deP1 = await _servicio.ListarAsync(p1.Id);

        Assert.Equal(new[] { "B", "C", "A" }, todos.Select(p => p.NumeroComprobante));
        Assert.All(todos, p => Assert.NotNull(p.Tratamiento?.Paciente));
        Assert.Equal(new[] { "C", "A" }, deP1.Select(p => p.NumeroComprobante));
    }

    [Fact]
    public async Task ObtenerPorId_IncluyeTratamientoYPaciente_ONull()
    {
        var t = await TratamientoDeAsync(100m);
        var pago = await _e.PagoAsync(t.Id, 10m);

        var r = await _servicio.ObtenerPorIdAsync(pago.Id);

        Assert.NotNull(r!.Tratamiento?.Paciente);
        Assert.Null(await _servicio.ObtenerPorIdAsync(999));
    }
}
