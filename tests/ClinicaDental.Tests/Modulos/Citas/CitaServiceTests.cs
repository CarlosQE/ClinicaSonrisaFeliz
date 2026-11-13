using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Common;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Modulos.Citas;

/// <summary>
/// Módulo 3 - Gestión de Citas (AndresFundamentalistas).
/// "Ahora" = lunes 07/01/2030 08:00. Odontólogos de prueba: L-V 08:00-12:00 y 14:00-18:00.
/// </summary>
[Trait("Modulo", Modulo.Citas)]
public class CitaServiceTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;
    private readonly CitaService _servicio;

    public CitaServiceTests()
    {
        _e = _bd.Escenario();
        _servicio = new CitaService(_bd.NuevoContexto(), _bd.Reloj);
    }

    public void Dispose() => _bd.Dispose();

    private Cita NuevaCita(int pacienteId, int odontologoId, DateTime fechaHora, int duracion = 30, string motivo = "Revisión")
        => new() { PacienteId = pacienteId, OdontologoId = odontologoId, FechaHora = fechaHora, DuracionMinutos = duracion, Motivo = motivo };

    // ================================================================== Agendar

    [Fact]
    public async Task Agendar_DatosValidos_GuardaCitaProgramada()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var fecha = _e.ProximoLunesA(9);

        var cita = await _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, fecha));

        Assert.True(cita.Id > 0);
        using var db = _bd.NuevoContexto();
        var guardada = await db.Citas.SingleAsync();
        Assert.Equal(EstadoCita.Programada, guardada.Estado);
        Assert.Equal(fecha, guardada.FechaHora);
        Assert.Equal(_bd.Reloj.Ahora, guardada.FechaRegistro);
        Assert.Equal("Revisión", guardada.Motivo);
    }

    [Fact]
    public async Task Agendar_PacienteInexistente_LanzaReglaNegocio()
    {
        var o = await _e.OdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(999, o.Id, _e.ProximoLunesA(9))));
    }

    [Fact]
    public async Task Agendar_OdontologoInexistente_LanzaReglaNegocio()
    {
        var p = await _e.PacienteAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, 999, _e.ProximoLunesA(9))));
    }

    [Fact]
    public async Task Agendar_OdontologoInactivo_LanzaReglaNegocio()
    {
        var p = await _e.PacienteAsync();
        var o = await _e.OdontologoAsync(activo: false);
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(9))));
        Assert.Contains("inactivo", ex.Message);
    }

    [Fact]
    public async Task Agendar_FechaPasada_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var ayer = _bd.Reloj.Ahora.AddDays(-3).Date.AddHours(9); // viernes anterior 09:00
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, ayer)));
    }

    [Fact]
    public async Task Agendar_ExactamenteAhora_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _bd.Reloj.Ahora)));
    }

    [Theory]
    [InlineData(12, 30)] // entre franjas (almuerzo)
    [InlineData(7, 0)]   // antes de abrir
    [InlineData(18, 0)]  // justo al cerrar
    [InlineData(11, 45)] // empieza dentro pero termina 12:15, fuera de la franja
    public async Task Agendar_FueraDeHorarioDelOdontologo_LanzaReglaNegocio(int hora, int minuto)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(hora, minuto))));
        Assert.Contains("horario", ex.Message);
    }

    [Fact]
    public async Task Agendar_DiaSinHorario_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var sabado = _e.ProximoLunes.AddDays(5).ToDateTime(new TimeOnly(9, 0));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, sabado)));
    }

    [Fact]
    public async Task Agendar_OdontologoSinNingunHorario_LanzaReglaNegocio()
    {
        var p = await _e.PacienteAsync();
        var o = await _e.OdontologoAsync(conHorario: false);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(9))));
    }

    [Fact]
    public async Task Agendar_JustoAlFinalDeLaFranja_EsValido()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(11, 30)));
        Assert.True(cita.Id > 0);
    }

    [Theory]
    [InlineData(9, 0)]   // misma hora
    [InlineData(9, 15)]  // empieza en medio
    [InlineData(8, 45)]  // termina en medio
    public async Task Agendar_SeSolapaConOtraCitaDelOdontologo_LanzaReglaNegocio(int hora, int minuto)
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9), duracion: 30);

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p2.Id, o.Id, _e.ProximoLunesA(hora, minuto))));
        Assert.Contains("ya tiene una cita", ex.Message);
    }

    [Fact]
    public async Task Agendar_CitaContigua_EsValida()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9), duracion: 30);

        var cita = await _servicio.AgendarAsync(NuevaCita(p2.Id, o.Id, _e.ProximoLunesA(9, 30)));
        Assert.True(cita.Id > 0);
    }

    [Fact]
    public async Task Agendar_HorarioDeCitaCancelada_EsValido()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9), estado: EstadoCita.Cancelada);

        var cita = await _servicio.AgendarAsync(NuevaCita(p2.Id, o.Id, _e.ProximoLunesA(9)));
        Assert.True(cita.Id > 0);
    }

    [Fact]
    public async Task Agendar_PacienteConOtraCitaSolapadaConOtroOdontologo_LanzaReglaNegocio()
    {
        var (p, o1) = await _e.PacienteYOdontologoAsync();
        var o2 = await _e.OdontologoAsync(nombres: "Luis");
        await _e.CitaAsync(p.Id, o1.Id, _e.ProximoLunesA(9), duracion: 60);

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o2.Id, _e.ProximoLunesA(9, 30))));
        Assert.Contains("paciente", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(241)]
    public async Task Agendar_DuracionInvalida_LanzaReglaNegocio(int minutos)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(9), minutos)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Agendar_SinMotivo_LanzaReglaNegocio(string motivo)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.AgendarAsync(NuevaCita(p.Id, o.Id, _e.ProximoLunesA(9), motivo: motivo)));
    }

    // ================================================================== Reprogramar

    [Fact]
    public async Task Reprogramar_AHorarioLibre_CambiaFechaYEstado()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        var nueva = _e.ProximoLunesA(15);

        await _servicio.ReprogramarAsync(cita.Id, nueva);

        using var db = _bd.NuevoContexto();
        var guardada = await db.Citas.SingleAsync();
        Assert.Equal(nueva, guardada.FechaHora);
        Assert.Equal(EstadoCita.Reprogramada, guardada.Estado);
    }

    [Fact]
    public async Task Reprogramar_SolapandoseConSuPropioHorarioAnterior_EsValido()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9), duracion: 60);

        var r = await _servicio.ReprogramarAsync(cita.Id, _e.ProximoLunesA(9, 30));
        Assert.Equal(_e.ProximoLunesA(9, 30), r.FechaHora);
    }

    [Fact]
    public async Task Reprogramar_CambiandoDeOdontologo_GuardaElNuevo()
    {
        var (p, o1) = await _e.PacienteYOdontologoAsync();
        var o2 = await _e.OdontologoAsync(nombres: "Luis");
        var cita = await _e.CitaAsync(p.Id, o1.Id, _e.ProximoLunesA(9));

        await _servicio.ReprogramarAsync(cita.Id, _e.ProximoLunesA(10), o2.Id);

        using var db = _bd.NuevoContexto();
        Assert.Equal(o2.Id, (await db.Citas.SingleAsync()).OdontologoId);
    }

    [Fact]
    public async Task Reprogramar_AHorarioOcupado_LanzaReglaNegocio()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        var cita = await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9));
        await _e.CitaAsync(p2.Id, o.Id, _e.ProximoLunesA(10));

        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ReprogramarAsync(cita.Id, _e.ProximoLunesA(10)));
    }

    [Fact]
    public async Task Reprogramar_AFechaPasada_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ReprogramarAsync(cita.Id, _bd.Reloj.Ahora.AddHours(-1)));
    }

    [Theory]
    [InlineData(EstadoCita.Cancelada)]
    [InlineData(EstadoCita.Atendida)]
    public async Task Reprogramar_CitaNoVigente_LanzaReglaNegocio(EstadoCita estado)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9), estado: estado);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.ReprogramarAsync(cita.Id, _e.ProximoLunesA(10)));
    }

    [Fact]
    public async Task Reprogramar_CitaInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ReprogramarAsync(999, _e.ProximoLunesA(10)));
    }

    // ================================================================== Cancelar

    [Fact]
    public async Task Cancelar_CitaVigente_GuardaEstadoYMotivo()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));

        await _servicio.CancelarAsync(cita.Id, "  Paciente enfermo  ");

        using var db = _bd.NuevoContexto();
        var guardada = await db.Citas.SingleAsync();
        Assert.Equal(EstadoCita.Cancelada, guardada.Estado);
        Assert.Equal("Paciente enfermo", guardada.MotivoCancelacion);
    }

    [Fact]
    public async Task Cancelar_LiberaElHorarioParaOtraCita()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        var cita = await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9));

        await _servicio.CancelarAsync(cita.Id, "No puede asistir");
        var nueva = await _servicio.AgendarAsync(NuevaCita(p2.Id, o.Id, _e.ProximoLunesA(9)));

        Assert.True(nueva.Id > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Cancelar_SinMotivo_LanzaReglaNegocio(string motivo)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CancelarAsync(cita.Id, motivo));
    }

    [Theory]
    [InlineData(EstadoCita.Cancelada)]
    [InlineData(EstadoCita.Atendida)]
    public async Task Cancelar_CitaNoVigente_LanzaReglaNegocio(EstadoCita estado)
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9), estado: estado);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.CancelarAsync(cita.Id, "x"));
    }

    [Fact]
    public async Task Cancelar_CitaInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.CancelarAsync(999, "x"));
    }

    // ================================================================== Marcar atendida

    [Fact]
    public async Task MarcarAtendida_CitaYaIniciada_CambiaEstado()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _bd.Reloj.Ahora.AddMinutes(-10));

        await _servicio.MarcarAtendidaAsync(cita.Id);

        using var db = _bd.NuevoContexto();
        Assert.Equal(EstadoCita.Atendida, (await db.Citas.SingleAsync()).Estado);
    }

    [Fact]
    public async Task MarcarAtendida_CitaFutura_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.MarcarAtendidaAsync(cita.Id));
    }

    [Fact]
    public async Task MarcarAtendida_CitaCancelada_LanzaReglaNegocio()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _bd.Reloj.Ahora.AddMinutes(-10), estado: EstadoCita.Cancelada);
        await Assert.ThrowsAsync<ReglaNegocioException>(() => _servicio.MarcarAtendidaAsync(cita.Id));
    }

    [Fact]
    public async Task MarcarAtendida_CitaInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.MarcarAtendidaAsync(999));
    }

    // ================================================================== Agenda diaria y consultas

    [Fact]
    public async Task AgendaDiaria_DevuelveSoloElDiaOrdenadoPorHoraYSinCanceladas()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var lunes = _e.ProximoLunes;
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(15), motivo: "tarde");
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9), motivo: "mañana");
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(10), estado: EstadoCita.Cancelada, motivo: "cancelada");
        await _e.CitaAsync(p.Id, o.Id, lunes.AddDays(1).ToDateTime(new TimeOnly(9, 0)), motivo: "martes");

        var agenda = await _servicio.ObtenerAgendaDiariaAsync(lunes);

        Assert.Equal(new[] { "mañana", "tarde" }, agenda.Select(c => c.Motivo));
        Assert.All(agenda, c =>
        {
            Assert.NotNull(c.Paciente);
            Assert.NotNull(c.Odontologo);
        });
    }

    [Fact]
    public async Task AgendaDiaria_IncluirCanceladas_LasDevuelve()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(10), estado: EstadoCita.Cancelada);

        var agenda = await _servicio.ObtenerAgendaDiariaAsync(_e.ProximoLunes, incluirCanceladas: true);
        Assert.Equal(2, agenda.Count);
    }

    [Fact]
    public async Task AgendaDiaria_FiltraPorOdontologo()
    {
        var (p, o1) = await _e.PacienteYOdontologoAsync();
        var o2 = await _e.OdontologoAsync(nombres: "Luis");
        await _e.CitaAsync(p.Id, o1.Id, _e.ProximoLunesA(9));
        await _e.CitaAsync(p.Id, o2.Id, _e.ProximoLunesA(15));

        var agenda = await _servicio.ObtenerAgendaDiariaAsync(_e.ProximoLunes, o2.Id);

        Assert.Single(agenda);
        Assert.Equal(o2.Id, agenda[0].OdontologoId);
    }

    [Fact]
    public async Task Listar_FiltraPorRangoEstadoYPaciente()
    {
        var (p1, o) = await _e.PacienteYOdontologoAsync();
        var p2 = await _e.PacienteAsync("Otro", "Paciente");
        var lunes = _e.ProximoLunes;
        await _e.CitaAsync(p1.Id, o.Id, _e.ProximoLunesA(9));
        await _e.CitaAsync(p1.Id, o.Id, lunes.AddDays(2).ToDateTime(new TimeOnly(9, 0)), estado: EstadoCita.Cancelada);
        await _e.CitaAsync(p2.Id, o.Id, lunes.AddDays(7).ToDateTime(new TimeOnly(9, 0)));

        Assert.Equal(2, (await _servicio.ListarAsync(lunes, lunes.AddDays(2))).Count);
        Assert.Single(await _servicio.ListarAsync(estado: EstadoCita.Cancelada));
        Assert.Single(await _servicio.ListarAsync(pacienteId: p2.Id));
        Assert.Equal(3, (await _servicio.ListarAsync()).Count);
    }

    [Fact]
    public async Task ObtenerPorId_IncluyePacienteYOdontologo_ONullSiNoExiste()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var cita = await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));

        var r = await _servicio.ObtenerPorIdAsync(cita.Id);

        Assert.NotNull(r);
        Assert.Equal(p.CI, r!.Paciente!.CI);
        Assert.Equal(o.Matricula, r.Odontologo!.Matricula);
        Assert.Null(await _servicio.ObtenerPorIdAsync(999));
    }

    // ================================================================== Horarios disponibles

    [Fact]
    public async Task HorariosDisponibles_DescuentaCitasOcupadas()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9), duracion: 60);              // ocupa 09:00 y 09:30
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(10), estado: EstadoCita.Cancelada); // no ocupa

        var libres = await _servicio.ObtenerHorariosDisponiblesAsync(o.Id, _e.ProximoLunes, 30);

        // 08:00-12:00 (8 bloques) + 14:00-18:00 (8 bloques) - 2 ocupados = 14
        Assert.Equal(14, libres.Count);
        Assert.DoesNotContain(_e.ProximoLunesA(9), libres);
        Assert.DoesNotContain(_e.ProximoLunesA(9, 30), libres);
        Assert.Contains(_e.ProximoLunesA(10), libres);
        Assert.Equal(libres.OrderBy(x => x), libres);
    }

    [Fact]
    public async Task HorariosDisponibles_HoyNoDevuelveHorasPasadas()
    {
        var o = await _e.OdontologoAsync();
        _bd.Reloj.Ahora = RelojFijo.LunesBase.AddHours(3); // lunes 11:00

        var libres = await _servicio.ObtenerHorariosDisponiblesAsync(o.Id, DateOnly.FromDateTime(RelojFijo.LunesBase), 30);

        Assert.All(libres, h => Assert.True(h > _bd.Reloj.Ahora));
        Assert.Equal(RelojFijo.LunesBase.Date.AddHours(11).AddMinutes(30), libres.First());
    }

    [Fact]
    public async Task HorariosDisponibles_OdontologoInactivo_ListaVacia()
    {
        var o = await _e.OdontologoAsync(activo: false);
        Assert.Empty(await _servicio.ObtenerHorariosDisponiblesAsync(o.Id, _e.ProximoLunes));
    }

    [Fact]
    public async Task HorariosDisponibles_OdontologoInexistente_LanzaNoEncontrado()
    {
        await Assert.ThrowsAsync<NoEncontradoException>(() => _servicio.ObtenerHorariosDisponiblesAsync(999, _e.ProximoLunes));
    }
}
