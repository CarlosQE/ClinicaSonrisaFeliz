using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClinicaDental.Tests.Infraestructura;

/// <summary>
/// Levanta la aplicación web COMPLETA en memoria (Program.cs real, controladores, vistas, filtros)
/// con una base SQLite en memoria propia, datos de ejemplo (DatosIniciales) y el reloj fijo.
/// Se usa en los tests de integración: si una página de tu módulo no responde 200 aquí,
/// tampoco funcionará en el programa unificado.
/// </summary>
public class ClinicaWebFactory : WebApplicationFactory<Program>
{
    private readonly string _cadena = $"Data Source=clinica-test-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _mantenerViva;

    public RelojFijo Reloj { get; } = new();

    public ClinicaWebFactory()
    {
        // Una base en memoria compartida existe mientras haya al menos una conexión abierta.
        _mantenerViva = new SqliteConnection(_cadena);
        _mantenerViva.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:ClinicaDb", _cadena);
        builder.UseSetting("BaseDeDatos:SembrarDatos", "true");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(Reloj);
        });
    }

    /// <summary>Cliente HTTP que NO sigue redirecciones (para poder verificar 302).</summary>
    public HttpClient ClienteSinRedireccion()
        => CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    /// <summary>Extrae el token antiforgery de un formulario HTML (necesario para hacer POST).</summary>
    public static string ExtraerTokenAntiforgery(string html)
    {
        var m = Regex.Match(html, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");
        if (!m.Success) throw new InvalidOperationException("No se encontró el token antiforgery en el formulario.");
        return m.Groups[1].Value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _mantenerViva.Dispose();
    }
}
