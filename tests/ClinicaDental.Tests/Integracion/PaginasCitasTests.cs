using System.Net;
using System.Text.Json;
using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Integracion;

/// <summary>
/// Integración del módulo 3 - Citas (AndresFundamentalistas): páginas + flujos completos por HTTP
/// (formulario -> POST -> redirección -> dato guardado). Sirve de ejemplo para los demás grupos.
/// Reloj de la app en estas pruebas: lunes 07/01/2030 08:00.
/// </summary>
[Trait("Modulo", Modulo.Citas)]
[Trait("Tipo", "Integracion")]
public class PaginasCitasTests : PruebaIntegracion
{
    public PaginasCitasTests(ClinicaWebFactory f) : base(f) { }

    private static readonly DateTime LunesSiguiente = RelojFijo.LunesBase.Date.AddDays(7); // 14/01/2030

    [Theory]
    [InlineData("/Citas")]
    [InlineData("/Citas?desde=2030-01-01&hasta=2030-12-31&estado=Programada")]
    [InlineData("/Citas/Agenda")]
    [InlineData("/Citas/Agenda?fecha=2030-01-08&incluirCanceladas=true")]
    [InlineData("/Citas/Create")]
    [InlineData("/Citas/Details/{cita}")]
    [InlineData("/Citas/Reprogramar/{citaVigente}")]
    [InlineData("/Citas/Cancelar/{citaVigente}")]
    public Task Pagina_Responde200(string ruta) => PaginaOkAsync(ruta);

    [Theory]
    [InlineData("/Citas/Details/99999")]
    [InlineData("/Citas/Reprogramar/99999")]
    [InlineData("/Citas/Cancelar/99999")]
    public Task IdInexistente_Devuelve404(string ruta) => NoEncontradoAsync(ruta);

    [Fact]
    public async Task AgendaDelMartes_MuestraLasCitasDeEjemplo()
    {
        var html = await PaginaOkAsync("/Citas/Agenda?fecha=2030-01-08");
        Assert.Contains("data-testid=\"tabla-agenda\"", html);
        Assert.Contains("Dolor de muela", html);
    }

    [Fact]
    public async Task HorariosDisponibles_DevuelveJson()
    {
        var anaId = ConBaseDeDatos(db => db.Odontologos.Single(o => o.Matricula == "MAT-1001").Id);

        var resp = await Cliente.GetAsync($"/Citas/HorariosDisponibles?odontologoId={anaId}&fecha=2030-01-18&duracion=60");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var horas = JsonSerializer.Deserialize<string[]>(await resp.Content.ReadAsStringAsync())!;
        Assert.Equal(8, horas.Length); // 4 bloques de 1h en la mañana + 4 en la tarde
        Assert.Equal("2030-01-18T08:00", horas[0]); // viernes sin citas
    }

    [Fact]
    public async Task Agendar_FormularioValido_GuardaYRedirigeALaAgenda()
    {
        var (pacienteId, odontologoId) = ConBaseDeDatos(db => (
            db.Pacientes.Single(p => p.CI == "2345678").Id,
            db.Odontologos.Single(o => o.Matricula == "MAT-1001").Id));

        var resp = await EnviarFormularioAsync("/Citas/Create", new()
        {
            ["PacienteId"] = pacienteId.ToString(),
            ["OdontologoId"] = odontologoId.ToString(),
            ["FechaHora"] = LunesSiguiente.AddHours(11).ToString("yyyy-MM-ddTHH:mm"),
            ["DuracionMinutos"] = "30",
            ["Motivo"] = "Prueba de integración"
        });

        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        Assert.StartsWith("/Citas/Agenda", resp.Headers.Location!.OriginalString);
        Assert.True(ConBaseDeDatos(db => db.Citas.Any(c => c.Motivo == "Prueba de integración" && c.Estado == EstadoCita.Programada)));
    }

    [Fact]
    public async Task Agendar_FueraDeHorario_MuestraElErrorEnElFormulario()
    {
        var (pacienteId, odontologoId) = ConBaseDeDatos(db => (
            db.Pacientes.Single(p => p.CI == "3456789").Id,
            db.Odontologos.Single(o => o.Matricula == "MAT-1001").Id));

        var resp = await EnviarFormularioAsync("/Citas/Create", new()
        {
            ["PacienteId"] = pacienteId.ToString(),
            ["OdontologoId"] = odontologoId.ToString(),
            ["FechaHora"] = LunesSiguiente.AddHours(13).ToString("yyyy-MM-ddTHH:mm"), // 13:00 = almuerzo
            ["DuracionMinutos"] = "30",
            ["Motivo"] = "No debería guardarse"
        });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Contains("fuera del horario", await resp.Content.ReadAsStringAsync());
        Assert.False(ConBaseDeDatos(db => db.Citas.Any(c => c.Motivo == "No debería guardarse")));
    }

    [Fact]
    public async Task Cancelar_ConMotivo_CambiaEstadoYRedirigeAlDetalle()
    {
        var citaId = ConBaseDeDatos(db => db.Citas.Single(c => c.Motivo == "Evaluación general").Id);

        var resp = await EnviarFormularioAsync($"/Citas/Cancelar/{citaId}", new() { ["Id"] = citaId.ToString(), ["Motivo"] = "Paciente de viaje" });

        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        var cita = ConBaseDeDatos(db => db.Citas.AsNoTracking().Single(c => c.Id == citaId));
        Assert.Equal(EstadoCita.Cancelada, cita.Estado);
        Assert.Equal("Paciente de viaje", cita.MotivoCancelacion);
    }

    [Fact]
    public async Task Reprogramar_AHorarioLibre_CambiaFechaYEstado()
    {
        var citaId = ConBaseDeDatos(db => db.Citas.Single(c => c.Motivo == "Limpieza").Id);
        var nueva = LunesSiguiente.AddDays(1).AddHours(15);

        var resp = await EnviarFormularioAsync($"/Citas/Reprogramar/{citaId}", new()
        {
            ["Id"] = citaId.ToString(),
            ["NuevaFechaHora"] = nueva.ToString("yyyy-MM-ddTHH:mm")
        });

        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        var cita = ConBaseDeDatos(db => db.Citas.AsNoTracking().Single(c => c.Id == citaId));
        Assert.Equal(nueva, cita.FechaHora);
        Assert.Equal(EstadoCita.Reprogramada, cita.Estado);
    }

    /// <summary>GET del formulario (para obtener el token antiforgery) + POST con los campos.</summary>
    private async Task<HttpResponseMessage> EnviarFormularioAsync(string ruta, Dictionary<string, string> campos)
    {
        var formulario = await Cliente.GetStringAsync(ruta);
        campos["__RequestVerificationToken"] = ClinicaWebFactory.ExtraerTokenAntiforgery(formulario);
        return await Cliente.PostAsync(ruta, new FormUrlEncodedContent(campos));
    }
}
