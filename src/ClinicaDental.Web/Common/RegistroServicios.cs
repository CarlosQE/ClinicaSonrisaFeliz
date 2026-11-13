using ClinicaDental.Web.Data;
using ClinicaDental.Web.Services;
using ClinicaDental.Web.Services.Contratos;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Common;

/// <summary>
/// Registro centralizado de dependencias. Dueño: grupo líder.
/// Los 5 servicios YA están registrados: los grupos NO necesitan tocar este archivo
/// ni Program.cs. Solo implementan la clase de su servicio.
/// </summary>
public static class RegistroServicios
{
    public static IServiceCollection AgregarClinicaDental(this IServiceCollection services, IConfiguration config)
    {
        var cadena = config.GetConnectionString("ClinicaDb") ?? "Data Source=clinica.db";
        services.AddDbContext<ClinicaDbContext>(o => o.UseSqlite(cadena));

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IPacienteService, PacienteService>();       // Módulo 1 - ApostolesEvoMorales
        services.AddScoped<IOdontologoService, OdontologoService>();   // Módulo 2 - BagayerosChaco
        services.AddScoped<ICitaService, CitaService>();               // Módulo 3 - AndresFundamentalistas
        services.AddScoped<ITratamientoService, TratamientoService>(); // Módulo 4 - Ultimo
        services.AddScoped<IPagoService, PagoService>();               // Módulo 5 - GuaposMisteriosos

        return services;
    }

    /// <summary>Crea la base si no existe y carga datos de ejemplo si está vacía.</summary>
    public static void PrepararBaseDeDatos(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClinicaDbContext>();
        db.Database.EnsureCreated();

        if (app.Configuration.GetValue("BaseDeDatos:SembrarDatos", true))
            DatosIniciales.Sembrar(db, scope.ServiceProvider.GetRequiredService<TimeProvider>());
    }
}
