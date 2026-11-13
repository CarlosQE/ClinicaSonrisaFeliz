using System.Net;
using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaDental.Tests.Integracion;

/// <summary>
/// Base para pruebas de integración: arranca la app completa (con datos de ejemplo)
/// y permite pedir páginas como lo haría el navegador.
///
/// En las rutas se pueden usar estos marcadores, que se reemplazan por ids reales de los datos de ejemplo:
///   {paciente} {odontologo} {cita} {citaVigente} {tratamiento} {pago}
/// </summary>
public abstract class PruebaIntegracion : IClassFixture<ClinicaWebFactory>
{
    protected readonly ClinicaWebFactory Fabrica;
    protected readonly HttpClient Cliente;

    protected PruebaIntegracion(ClinicaWebFactory fabrica)
    {
        Fabrica = fabrica;
        Cliente = fabrica.ClienteSinRedireccion();
    }

    /// <summary>Ejecuta código con el DbContext real de la app (para preparar o verificar datos).</summary>
    protected T ConBaseDeDatos<T>(Func<ClinicaDbContext, T> accion)
    {
        using var scope = Fabrica.Services.CreateScope();
        return accion(scope.ServiceProvider.GetRequiredService<ClinicaDbContext>());
    }

    protected string ResolverRuta(string ruta) => ConBaseDeDatos(db => ruta
        .Replace("{paciente}", db.Pacientes.OrderBy(x => x.Id).First().Id.ToString())
        .Replace("{odontologo}", db.Odontologos.OrderBy(x => x.Id).First().Id.ToString())
        .Replace("{cita}", db.Citas.OrderBy(x => x.Id).First().Id.ToString())
        .Replace("{citaVigente}", db.Citas.Where(c => c.Estado == EstadoCita.Programada).OrderBy(x => x.Id).First().Id.ToString())
        .Replace("{tratamiento}", db.Tratamientos.OrderBy(x => x.Id).First().Id.ToString())
        .Replace("{pago}", db.Pagos.OrderBy(x => x.Id).First().Id.ToString()));

    /// <summary>GET a la ruta y verifica 200 OK + que se renderizó con el layout común.</summary>
    protected async Task<string> PaginaOkAsync(string ruta)
    {
        var url = ResolverRuta(ruta);
        var resp = await Cliente.GetAsync(url);
        var html = await resp.Content.ReadAsStringAsync();

        Assert.True(resp.StatusCode == HttpStatusCode.OK,
            $"GET {url} devolvió {(int)resp.StatusCode} {resp.StatusCode}. " +
            (resp.StatusCode == HttpStatusCode.NotImplemented ? "El módulo todavía lanza NotImplementedException." :
             resp.StatusCode == HttpStatusCode.NotFound ? "No existe la acción/vista o el id no se encontró." : ""));
        Assert.Contains("data-testid=\"menu-principal\"", html); // usa _Layout.cshtml
        return html;
    }

    protected async Task NoEncontradoAsync(string ruta)
    {
        var resp = await Cliente.GetAsync(ResolverRuta(ruta));
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }
}
