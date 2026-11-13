using System.Net;
using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Services.Contratos;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaDental.Tests.Integracion;

// =============================================================================================
//  Pruebas de INTEGRACIÓN: levantan la aplicación completa y piden cada página obligatoria.
//  Garantizan que, al unir todos los módulos, el menú, las rutas y las vistas funcionan.
//  Las rutas obligatorias de cada módulo están documentadas en el README y en cada controlador.
// =============================================================================================

[Trait("Modulo", Modulo.Base)]
[Trait("Tipo", "Integracion")]
public class PaginasBaseTests : PruebaIntegracion
{
    public PaginasBaseTests(ClinicaWebFactory f) : base(f) { }

    [Fact]
    public async Task Inicio_MuestraMenuConLosCincoModulos()
    {
        var html = await PaginaOkAsync("/");
        Assert.Contains("href=\"/Pacientes\"", html);
        Assert.Contains("href=\"/Odontologos\"", html);
        Assert.Contains("href=\"/Citas/Agenda\"", html);
        Assert.Contains("href=\"/Tratamientos\"", html);
        Assert.Contains("href=\"/Pagos\"", html);
    }

    [Theory]
    [InlineData(typeof(IPacienteService))]
    [InlineData(typeof(IOdontologoService))]
    [InlineData(typeof(ICitaService))]
    [InlineData(typeof(ITratamientoService))]
    [InlineData(typeof(IPagoService))]
    public void TodosLosServiciosEstanRegistrados(Type contrato)
    {
        using var scope = Fabrica.Services.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService(contrato));
    }

    [Fact]
    public async Task RutaInexistente_Devuelve404()
    {
        var resp = await Cliente.GetAsync("/NoExiste/Nada");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }
}

[Trait("Modulo", Modulo.Pacientes)]
[Trait("Tipo", "Integracion")]
public class PaginasPacientesTests : PruebaIntegracion
{
    public PaginasPacientesTests(ClinicaWebFactory f) : base(f) { }

    [Theory]
    [InlineData("/Pacientes")]
    [InlineData("/Pacientes?busqueda=a")]
    [InlineData("/Pacientes/Create")]
    [InlineData("/Pacientes/Edit/{paciente}")]
    [InlineData("/Pacientes/Historial/{paciente}")]
    public Task Pagina_Responde200(string ruta) => PaginaOkAsync(ruta);

    [Theory]
    [InlineData("/Pacientes/Edit/99999")]
    [InlineData("/Pacientes/Historial/99999")]
    public Task IdInexistente_Devuelve404(string ruta) => NoEncontradoAsync(ruta);
}

[Trait("Modulo", Modulo.Odontologos)]
[Trait("Tipo", "Integracion")]
public class PaginasOdontologosTests : PruebaIntegracion
{
    public PaginasOdontologosTests(ClinicaWebFactory f) : base(f) { }

    [Theory]
    [InlineData("/Odontologos")]
    [InlineData("/Odontologos/Create")]
    [InlineData("/Odontologos/Edit/{odontologo}")]
    [InlineData("/Odontologos/Horarios/{odontologo}")]
    [InlineData("/Odontologos/Especialidades")]
    public Task Pagina_Responde200(string ruta) => PaginaOkAsync(ruta);

    [Theory]
    [InlineData("/Odontologos/Edit/99999")]
    [InlineData("/Odontologos/Horarios/99999")]
    public Task IdInexistente_Devuelve404(string ruta) => NoEncontradoAsync(ruta);
}

[Trait("Modulo", Modulo.Tratamientos)]
[Trait("Tipo", "Integracion")]
public class PaginasTratamientosTests : PruebaIntegracion
{
    public PaginasTratamientosTests(ClinicaWebFactory f) : base(f) { }

    [Theory]
    [InlineData("/Tratamientos")]
    [InlineData("/Tratamientos?pacienteId={paciente}")]
    [InlineData("/Tratamientos/Create")]
    [InlineData("/Tratamientos/Create?citaId={cita}")]
    [InlineData("/Tratamientos/Details/{tratamiento}")]
    [InlineData("/Tratamientos/Observaciones/{tratamiento}")]
    public Task Pagina_Responde200(string ruta) => PaginaOkAsync(ruta);

    [Theory]
    [InlineData("/Tratamientos/Details/99999")]
    [InlineData("/Tratamientos/Observaciones/99999")]
    public Task IdInexistente_Devuelve404(string ruta) => NoEncontradoAsync(ruta);
}

[Trait("Modulo", Modulo.Pagos)]
[Trait("Tipo", "Integracion")]
public class PaginasPagosTests : PruebaIntegracion
{
    public PaginasPagosTests(ClinicaWebFactory f) : base(f) { }

    [Theory]
    [InlineData("/Pagos")]
    [InlineData("/Pagos?pacienteId={paciente}")]
    [InlineData("/Pagos/Create")]
    [InlineData("/Pagos/Create?tratamientoId={tratamiento}")]
    [InlineData("/Pagos/Deudas")]
    [InlineData("/Pagos/Comprobante/{pago}")]
    public Task Pagina_Responde200(string ruta) => PaginaOkAsync(ruta);

    [Fact]
    public Task ComprobanteInexistente_Devuelve404() => NoEncontradoAsync("/Pagos/Comprobante/99999");
}
