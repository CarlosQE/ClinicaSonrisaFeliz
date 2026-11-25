using System.Text;
using System.Threading.RateLimiting;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Interfaces;
using ClinicaDental.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var db = builder.Configuration.GetConnectionString("ConexionDB");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(db));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    // Botón "Authorize" en Swagger: pega el token que devuelve /api/Auth/login
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aquí el token que devuelve POST /api/Auth/login"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// ---------------------------------------------------------------------------
// Seguridad: autenticación con JWT (Bearer). Configuración en appsettings.json → "Jwt".
// En producción la clave debe venir de una variable de entorno o de User Secrets, no del repositorio.
// ---------------------------------------------------------------------------
var claveJwt = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrWhiteSpace(claveJwt) || Encoding.UTF8.GetByteCount(claveJwt) < 32)
{
    throw new InvalidOperationException("Falta Jwt:Clave en la configuración o tiene menos de 32 caracteres.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Límite de intentos de inicio de sesión por dirección IP (protección contra fuerza bruta).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", contexto => RateLimitPartition.GetFixedWindowLimiter(
        contexto.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    options.OnRejected = async (contexto, token) =>
    {
        contexto.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
        await contexto.HttpContext.Response.WriteAsync("Demasiados intentos de inicio de sesión. Espera un minuto e inténtalo de nuevo.", token);
    };
});

// Inyección de dependencias de servicios.
// Cada grupo descomenta SOLO la línea de su módulo (no toca las de los demás).

// Seguridad - Autenticación y usuarios (grupo líder)
builder.Services.AddScoped<IAuth, AuthServicio>();

// Módulo 1 - Pacientes (ApostolesEvoMorales)
builder.Services.AddScoped<IPaciente, PacienteServicio>();

// Módulo 2 - Odontólogos (BagayerosChaco)
builder.Services.AddScoped<IOdontologo, OdontologoServicio>();
builder.Services.AddScoped<IEspecialidad, EspecialidadServicio>();
builder.Services.AddScoped<IHorarioOdontologo, HorarioOdontologoServicio>();

// Módulo 3 - Citas (AndresFundamentalistas)
builder.Services.AddScoped<ICita, CitaServicio>();

// Módulo 4 - Tratamientos (Ultimo)
builder.Services.AddScoped<ITratamiento, TratamientoServicio>();

// Módulo 5 - Pagos (GuaposMisteriosos)
builder.Services.AddScoped<IPago, PagoServicio>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<BaseDatos>();
    contexto.Database.Migrate();

    if (builder.Configuration.GetValue("Siembra:Habilitada", true))
    {
        var cantidadExtra = builder.Configuration.GetValue("Siembra:CantidadExtra", 1000);
        DatosIniciales.Sembrar(contexto, cantidadExtra);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

// Todos los endpoints exigen un token válido salvo los marcados con [AllowAnonymous] (POST /api/Auth/login).
app.MapControllers().RequireAuthorization();

app.Run();

/// <summary>Necesario para que los tests de integración (WebApplicationFactory) puedan arrancar la app.</summary>
public partial class Program { }
