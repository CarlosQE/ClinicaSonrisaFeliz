using ClinicaDental.Web.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Infraestructura;

/// <summary>
/// Base de datos SQLite EN MEMORIA, nueva y vacía para cada test (mismo motor que la app real,
/// así que se respetan claves foráneas e índices únicos).
///
/// Uso típico:
///   using var bd = new BaseDatosPrueba();
///   var servicio = new MiServicio(bd.NuevoContexto(), bd.Reloj);
///   ...
///   using var verificacion = bd.NuevoContexto();   // contexto limpio para comprobar lo guardado
/// </summary>
public sealed class BaseDatosPrueba : IDisposable
{
    private readonly SqliteConnection _conexion;
    private readonly DbContextOptions<ClinicaDbContext> _opciones;

    public RelojFijo Reloj { get; } = new();

    public BaseDatosPrueba()
    {
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();
        _opciones = new DbContextOptionsBuilder<ClinicaDbContext>().UseSqlite(_conexion).Options;

        using var db = NuevoContexto();
        db.Database.EnsureCreated();
    }

    /// <summary>Crea un DbContext nuevo sobre la misma base en memoria.</summary>
    public ClinicaDbContext NuevoContexto() => new(_opciones);

    /// <summary>Atajo para preparar datos con <see cref="Escenario"/>.</summary>
    public Escenario Escenario() => new(this);

    public void Dispose() => _conexion.Dispose();
}
