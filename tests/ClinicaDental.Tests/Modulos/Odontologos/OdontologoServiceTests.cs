using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Common;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Modulos.Odontologos;

/// <summary>
/// Módulo 2 - Gestión de Odontólogos (BagayerosChaco).
/// Estas pruebas definen el comportamiento esperado de IOdontologoService.
/// Al inicio TODAS fallan (NotImplementedException). El módulo está terminado cuando todas pasan:
///     dotnet test --filter "Modulo=Odontologos"
/// NO modifiques estas pruebas para que pasen: modifica OdontologoService.cs.
/// </summary>
[Trait("Modulo", Modulo.Odontologos)]
public class OdontologoServiceTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;
    private readonly OdontologoService _servicio;

    public OdontologoServiceTests()
    {
        _e = _bd.Escenario();
        _servicio = new OdontologoService(_bd.NuevoContexto());
    }

    public void Dispose() => _bd.Dispose();

    private static Odontologo Nuevo(int especialidadId, string matricula = "MAT-9000", string nombres = "Laura", string apellidos = "Céspedes")
        => new() { Nombres = nombres, Apellidos = apellidos, Matricula = matricula, Telefono = "71234567", EspecialidadId = especialidadId };

    private static HorarioOdontologo Franja(int odontologoId, DayOfWeek dia, int desde, int hasta)
        => new() { OdontologoId = odontologoId, DiaSemana = dia, HoraInicio = new TimeOnly(desde, 0), HoraFin = new TimeOnly(hasta, 0) };

    // ------------------------------------------------------------------ Registrar / Actualizar

    [Fact]
    public async Task Registrar_DatosValidos_GuardaActivo()
    {
        var esp = await _e.EspecialidadAsync();
        var nuevo = Nuevo(esp.Id);
        nuevo.Activo = false; // el servicio debe forzar Activo = true

        var o = await _servicio.RegistrarAsync(nuevo);

        Assert.True(o.Id > 0);
        using var db = _bd.NuevoContexto();
        Assert.True((await db.Odontologos.SingleAsync()).Activo);
    }

    [Fact]
    public async Task Registrar_MatriculaDuplicada_LanzaReglaNegocio()
    {
        var existente = await _e.OdontologoAsync(matricula: "MAT-9000");
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(existente.EspecialidadId, "MAT-9000")));
    }

    [Fact]
    public async Task Registrar_EspecialidadInexistente_LanzaReglaNegocio()
    {
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarAsync(Nuevo(999)));
    }

    [Fact]
    public async Task Actualizar_DatosValidos_GuardaCambios()
    {
        var o = await _e.OdontologoAsync(matricula: "MAT-1");
        var otraEsp = await _e.EspecialidadAsync("Periodoncia");
        var cambios = Nuevo(otraEsp.Id, "MAT-2", "Cambiado");
        cambios.Id = o.Id;

        await _servicio.ActualizarAsync(cambios);

        using var db = _bd.NuevoContexto();
        var guardado = await db.Odontologos.SingleAsync();
        Assert.Equal("MAT-2", guardado.Matricula);
        Assert.Equal("Cambiado", guardado.Nombres);
        Assert.Equal(otraEsp.Id, guardado.EspecialidadId);
    }

    [Fact]
    public async Task Actualizar_MatriculaDeOtro_LanzaReglaNegocio()
    {
        await _e.OdontologoAsync(matricula: "MAT-1");
        var o2 = await _e.OdontologoAsync(matricula: "MAT-2");
        var cambios = Nuevo(o2.EspecialidadId, "MAT-1");
        cambios.Id = o2.Id;

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ActualizarAsync(cambios));
    }

    [Fact]
    public async Task Actualizar_Inexistente_LanzaNoEncontrado()
    {
        var esp = await _e.EspecialidadAsync();
        var cambios = Nuevo(esp.Id);
        cambios.Id = 999;
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ActualizarAsync(cambios));
    }

    // ------------------------------------------------------------------ Estado activo / inactivo

    [Fact]
    public async Task CambiarEstado_DesactivaYActiva()
    {
        var o = await _e.OdontologoAsync();

        await _servicio.CambiarEstadoAsync(o.Id, false);
        using (var db = _bd.NuevoContexto()) Assert.False((await db.Odontologos.SingleAsync()).Activo);

        await _servicio.CambiarEstadoAsync(o.Id, true);
        using (var db = _bd.NuevoContexto()) Assert.True((await db.Odontologos.SingleAsync()).Activo);
    }

    [Fact]
    public async Task CambiarEstado_Inexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.CambiarEstadoAsync(999, false));
    }

    // ------------------------------------------------------------------ Listar / Obtener

    [Fact]
    public async Task Listar_OrdenadoIncluyeEspecialidadYFiltraActivos()
    {
        await _e.OdontologoAsync(nombres: "Zulma", apellidos: "Arias");
        await _e.OdontologoAsync(nombres: "Bruno", apellidos: "Zeballos", activo: false);
        await _e.OdontologoAsync(nombres: "Ana", apellidos: "Arias");

        var todos = await _servicio.ListarAsync();
        var activos = await _servicio.ListarAsync(soloActivos: true);

        Assert.Equal(new[] { "Ana", "Zulma", "Bruno" }, todos.Select(o => o.Nombres));
        Assert.All(todos, o => Assert.NotNull(o.Especialidad));
        Assert.Equal(2, activos.Count);
        Assert.All(activos, o => Assert.True(o.Activo));
    }

    [Fact]
    public async Task ObtenerPorId_IncluyeEspecialidadYHorarios_ONull()
    {
        var o = await _e.OdontologoAsync();

        var r = await _servicio.ObtenerPorIdAsync(o.Id);

        Assert.NotNull(r!.Especialidad);
        Assert.Equal(10, r.Horarios.Count);
        Assert.Null(await _servicio.ObtenerPorIdAsync(999));
    }

    // ------------------------------------------------------------------ Especialidades

    [Fact]
    public async Task RegistrarEspecialidad_YListarOrdenadas()
    {
        await _servicio.RegistrarEspecialidadAsync(new Especialidad { Nombre = "Periodoncia" });
        await _servicio.RegistrarEspecialidadAsync(new Especialidad { Nombre = "Endodoncia" });

        var lista = await _servicio.ListarEspecialidadesAsync();

        Assert.Equal(new[] { "Endodoncia", "Periodoncia" }, lista.Select(e => e.Nombre));
    }

    [Fact]
    public async Task RegistrarEspecialidad_NombreRepetidoSinImportarMayusculas_LanzaReglaNegocio()
    {
        await _e.EspecialidadAsync("Ortodoncia");
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.RegistrarEspecialidadAsync(new Especialidad { Nombre = "ORTODONCIA" }));
    }

    // ------------------------------------------------------------------ Horarios de trabajo

    [Fact]
    public async Task AgregarHorario_Valido_SeGuarda()
    {
        var o = await _e.OdontologoAsync(conHorario: false);

        var h = await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 8, 12));

        Assert.True(h.Id > 0);
        using var db = _bd.NuevoContexto();
        Assert.Single(db.HorariosOdontologo);
    }

    [Theory]
    [InlineData(12, 12)]
    [InlineData(12, 8)]
    public async Task AgregarHorario_FinNoPosteriorAInicio_LanzaReglaNegocio(int desde, int hasta)
    {
        var o = await _e.OdontologoAsync(conHorario: false);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, desde, hasta)));
    }

    [Fact]
    public async Task AgregarHorario_SolapadoMismoDia_LanzaReglaNegocio()
    {
        var o = await _e.OdontologoAsync(conHorario: false);
        await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 8, 12));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 11, 14)));
    }

    [Fact]
    public async Task AgregarHorario_ContiguoUOtroDia_EsValido()
    {
        var o = await _e.OdontologoAsync(conHorario: false);
        await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 8, 12));
        await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 12, 14));
        await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Tuesday, 8, 12));

        var lista = await _servicio.ListarHorariosAsync(o.Id);

        Assert.Equal(3, lista.Count);
        Assert.Equal(lista.OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio).Select(h => h.Id), lista.Select(h => h.Id));
    }

    [Fact]
    public async Task AgregarHorario_OdontologoInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.AgregarHorarioAsync(Franja(999, DayOfWeek.Monday, 8, 12)));
    }

    [Fact]
    public async Task EliminarHorario_LoBorra_EInexistenteLanzaNoEncontrado()
    {
        var o = await _e.OdontologoAsync(conHorario: false);
        var h = await _servicio.AgregarHorarioAsync(Franja(o.Id, DayOfWeek.Monday, 8, 12));

        await _servicio.EliminarHorarioAsync(h.Id);

        using var db = _bd.NuevoContexto();
        Assert.Empty(db.HorariosOdontologo);
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.EliminarHorarioAsync(999));
    }
}
