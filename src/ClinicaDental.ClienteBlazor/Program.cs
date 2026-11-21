using System.Globalization;
using ClinicaDental.ClienteBlazor.Components;
using ClinicaDental.ClienteBlazor.Servicios;
using Microsoft.AspNetCore.Components.Authorization;

// ============================================================================
//  Cliente web (frontend) de la Clínica Dental Sonrisa Feliz — Blazor Web App.
//  Componentes .razor interactivos en el servidor que consumen la API REST de
//  ClinicaDental.Web con HttpClient y un token JWT obtenido al iniciar sesión.
// ============================================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(TimeProvider.System);

// HttpClient con nombre "Api": la URL base sale de appsettings.json (ApiSettings:BaseUrl).
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("Falta ApiSettings:BaseUrl en appsettings.json");
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.EndsWith('/') ? apiBaseUrl : apiBaseUrl + "/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Sesión y autenticación (un juego de servicios por usuario conectado).
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProveedorAutenticacion>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ProveedorAutenticacion>());

builder.Services.AddScoped<ClienteApi>();
builder.Services.AddScoped<Notificaciones>();
builder.Services.AddScoped<ServicioDialogo>();

var app = builder.Build();

// Fechas en español, pero números con PUNTO decimal en todas las PCs.
var cultura = (CultureInfo)new CultureInfo("es-BO").Clone();
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.NumberGroupSeparator = ",";
cultura.NumberFormat.CurrencyDecimalSeparator = ".";
cultura.NumberFormat.CurrencyGroupSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Cabeceras de seguridad básicas para todas las respuestas.
app.Use(async (contexto, siguiente) =>
{
    contexto.Response.Headers["X-Content-Type-Options"] = "nosniff";
    contexto.Response.Headers["X-Frame-Options"] = "DENY";
    contexto.Response.Headers["Referrer-Policy"] = "no-referrer";
    await siguiente();
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
