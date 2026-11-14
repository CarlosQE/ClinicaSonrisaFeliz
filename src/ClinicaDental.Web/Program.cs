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
// Inyección de dependencias de servicios (cada grupo agrega la línea de su módulo)
builder.Services.AddScoped<ICita, CitaServicio>();
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
