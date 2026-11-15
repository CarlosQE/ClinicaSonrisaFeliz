using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var db = builder.Configuration.GetConnectionString("ConexionDB");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(db));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Inyección de dependencias de servicios.
// Cada grupo descomenta SOLO la línea de su módulo (no toca las de los demás).

// Módulo 1 - Pacientes (ApostolesEvoMorales)
builder.Services.AddScoped<IPaciente, PacienteServicio>();

// Módulo 2 - Odontólogos (BagayerosChaco)
builder.Services.AddScoped<IOdontologo, OdontologoServicio>();

// Módulo 3 - Citas (AndresFundamentalistas)
builder.Services.AddScoped<ICita, CitaServicio>();

// Módulo 4 - Tratamientos (Ultimo)
builder.Services.AddScoped<ITratamiento, TratamientoServicio>();

// Módulo 5 - Pagos (GuaposMisteriosos)
builder.Services.AddScoped<IPago, PagoServicio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>Necesario para que los tests de integración (WebApplicationFactory) puedan arrancar la app.</summary>
public partial class Program { }
