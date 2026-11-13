using System.Globalization;
using ClinicaDental.Web.Common;
using Microsoft.AspNetCore.Localization;

// ============================================================================
//  Program.cs - Dueño: grupo líder. Los grupos NO deberían necesitar modificarlo.
//  Los servicios de cada módulo se registran en Common/RegistroServicios.cs
// ============================================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(o => o.Filters.Add<FiltroExcepcionesDominio>());
builder.Services.AgregarClinicaDental(builder.Configuration);

var app = builder.Build();

app.PrepararBaseDeDatos();

// Cultura única para todo el sistema, igual en todas las PCs: fechas en español (dd/MM/yyyy)
// pero números con PUNTO decimal (250.50), porque <input type="number"> siempre envía punto.
var cultura = (CultureInfo)new CultureInfo("es-BO").Clone();
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.NumberGroupSeparator = ",";
cultura.NumberFormat.CurrencyDecimalSeparator = ".";
cultura.NumberFormat.CurrencyGroupSeparator = ",";
cultura.NumberFormat.CurrencySymbol = "Bs";
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/// <summary>Necesario para que los tests de integración (WebApplicationFactory) puedan arrancar la app.</summary>
public partial class Program { }
