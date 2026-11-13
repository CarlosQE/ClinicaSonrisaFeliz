using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Common;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Modulos.Pacientes;

/// <summary>
/// Módulo 1 - Gestión de Pacientes (ApostolesEvoMorales).
/// Estas pruebas definen el comportamiento esperado de IPacienteService.
/// Al inicio TODAS fallan (NotImplementedException). El módulo está terminado cuando todas pasan:
///     dotnet test --filter "Modulo=Pacientes"
/// NO modifiques estas pruebas para que pasen: modifica PacienteService.cs.
/// </summary>
[Trait("Modulo", Modulo.Pacientes)]
public class PacienteServiceTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;
    private readonly PacienteService _servicio;

    public PacienteServiceTests()
    {
        _e = _bd.Escenario();
        _servicio = new PacienteService(_bd.NuevoContexto(), _bd.Reloj);
    }

    public void Dispose() => _bd.Dispose();

    private static Paciente Nuevo(string ci = "9876543", string nombres = "Rosa", string apellidos = "Quispe")
        => new()
        {
            Nombres = nombres,
            Apellidos = apellidos,
            CI = ci,
            FechaNacimiento = new DateOnly(1995, 3, 10),
            Telefono = "76543210",
            Email = "rosa@correo.com"
        };

    // ------------------------------------------------------------------ Registrar

    [Fact]
    public async Task Registrar_DatosValidos_GuardaConIdYFechaRegistro()
    {
        var p = await _servicio.RegistrarAsync(Nuevo());

        Assert.True(p.Id > 0);
        using var db = _bd.NuevoContexto();
        var guardado = await db.Pacientes.SingleAsync();
        Assert.Equal("9876543", guardado.CI);
        Assert.Equal(_bd.Reloj.Ahora, guardado.FechaRegistro);
    }

    [Fact]
    public async Task Registrar_CIDuplicado_LanzaReglaNegocio()
    {
        await _e.PacienteAsync(ci: "9876543");
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(ci: "9876543")));
    }

    [Fact]
    public async Task Registrar_FechaNacimientoFutura_LanzaReglaNegocio()
    {
        var p = Nuevo();
        p.FechaNacimiento = DateOnly.FromDateTime(_bd.Reloj.Ahora).AddDays(1);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(p));
    }

    // ------------------------------------------------------------------ Listar / Obtener

    [Fact]
    public async Task Listar_OrdenaPorApellidosYNombres()
    {
        await _e.PacienteAsync("Zoe", "Bravo");
        await _e.PacienteAsync("Ana", "Bravo");
        await _e.PacienteAsync("Carlos", "Arce");

        var lista = await _servicio.ListarAsync();

        Assert.Equal(new[] { "Carlos Arce", "Ana Bravo", "Zoe Bravo" }, lista.Select(p => p.NombreCompleto));
    }

    [Theory]
    [InlineData("mar")]      // parte del nombre, minúsculas
    [InlineData("GUTI")]     // parte del apellido, mayúsculas
    [InlineData("555")]      // parte del CI
    public async Task Listar_ConBusqueda_FiltraPorNombreApellidoOCI(string texto)
    {
        await _e.PacienteAsync("María", "Gutiérrez", ci: "5551234");
        await _e.PacienteAsync("Pedro", "Lima", ci: "1112223");

        var lista = await _servicio.ListarAsync(texto);

        Assert.Single(lista);
        Assert.Equal("5551234", lista[0].CI);
    }

    [Fact]
    public async Task Listar_BusquedaVacia_DevuelveTodos()
    {
        await _e.PacienteAsync();
        await _e.PacienteAsync();
        Assert.Equal(2, (await _servicio.ListarAsync("  ")).Count);
    }

    [Fact]
    public async Task ObtenerPorId_Existente_LoDevuelve_InexistenteNull()
    {
        var p = await _e.PacienteAsync(ci: "123");
        Assert.Equal("123", (await _servicio.ObtenerPorIdAsync(p.Id))!.CI);
        Assert.Null(await _servicio.ObtenerPorIdAsync(999));
    }

    // ------------------------------------------------------------------ Actualizar

    [Fact]
    public async Task Actualizar_DatosValidos_GuardaCambios()
    {
        var p = await _e.PacienteAsync(ci: "111");
        var cambios = Nuevo(ci: "222", nombres: "Nuevo", apellidos: "Nombre");
        cambios.Id = p.Id;
        cambios.Direccion = "Calle 1";

        await _servicio.ActualizarAsync(cambios);

        using var db = _bd.NuevoContexto();
        var guardado = await db.Pacientes.SingleAsync();
        Assert.Equal("222", guardado.CI);
        Assert.Equal("Nuevo", guardado.Nombres);
        Assert.Equal("Calle 1", guardado.Direccion);
    }

    [Fact]
    public async Task Actualizar_ConservandoSuPropioCI_EsValido()
    {
        var p = await _e.PacienteAsync(ci: "111");
        var cambios = Nuevo(ci: "111", nombres: "Cambiado");
        cambios.Id = p.Id;

        await _servicio.ActualizarAsync(cambios);

        using var db = _bd.NuevoContexto();
        Assert.Equal("Cambiado", (await db.Pacientes.SingleAsync()).Nombres);
    }

    [Fact]
    public async Task Actualizar_CIDeOtroPaciente_LanzaReglaNegocio()
    {
        await _e.PacienteAsync(ci: "111");
        var p2 = await _e.PacienteAsync(ci: "222");
        var cambios = Nuevo(ci: "111");
        cambios.Id = p2.Id;

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ActualizarAsync(cambios));
    }

    [Fact]
    public async Task Actualizar_Inexistente_LanzaNoEncontrado()
    {
        var cambios = Nuevo();
        cambios.Id = 999;
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ActualizarAsync(cambios));
    }

    // ------------------------------------------------------------------ Eliminar

    [Fact]
    public async Task Eliminar_SinHistorial_LoBorra()
    {
        var p = await _e.PacienteAsync();
        await _servicio.EliminarAsync(p.Id);

        using var db = _bd.NuevoContexto();
        Assert.Empty(db.Pacientes);
    }

    [Fact]
    public async Task Eliminar_ConCitas_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.EliminarAsync(p.Id));
    }

    [Fact]
    public async Task Eliminar_ConTratamientos_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await _e.TratamientoAsync(p.Id, o.Id);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.EliminarAsync(p.Id));
    }

    [Fact]
    public async Task Eliminar_Inexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.EliminarAsync(999));
    }

    // ------------------------------------------------------------------ Historial

    [Fact]
    public async Task Historial_DevuelveCitasYTratamientosDelPacienteDelMasRecienteAlMasAntiguo()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var otro = await _e.PacienteAsync("Otro", "Paciente");
        var lunes = _e.ProximoLunes;
        await _e.CitaAsync(p.Id, o.Id, lunes.ToDateTime(new TimeOnly(9, 0)), motivo: "primera");
        await _e.CitaAsync(p.Id, o.Id, lunes.AddDays(1).ToDateTime(new TimeOnly(9, 0)), estado: EstadoCita.Cancelada, motivo: "segunda");
        await _e.CitaAsync(otro.Id, o.Id, lunes.ToDateTime(new TimeOnly(10, 0)), motivo: "ajena");
        await _e.TratamientoAsync(p.Id, o.Id, fecha: new DateOnly(2029, 5, 1), tipo: TipoTratamiento.Extraccion);
        await _e.TratamientoAsync(p.Id, o.Id, fecha: new DateOnly(2029, 12, 1), tipo: TipoTratamiento.Endodoncia);

        var h = await _servicio.ObtenerHistorialAsync(p.Id);

        Assert.Equal(p.CI, h.Paciente.CI);
        Assert.Equal(new[] { "segunda", "primera" }, h.Citas.Select(c => c.Motivo));
        Assert.Equal(new[] { TipoTratamiento.Endodoncia, TipoTratamiento.Extraccion }, h.Tratamientos.Select(t => t.Tipo));
        Assert.All(h.Citas, c => Assert.NotNull(c.Odontologo));
        Assert.All(h.Tratamientos, t => Assert.NotNull(t.Odontologo));
    }

    [Fact]
    public async Task Historial_PacienteInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ObtenerHistorialAsync(999));
    }
}
