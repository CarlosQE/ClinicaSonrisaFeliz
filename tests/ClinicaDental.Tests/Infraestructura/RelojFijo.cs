namespace ClinicaDental.Tests.Infraestructura;

/// <summary>
/// Reloj "congelado" para que las pruebas no dependan del día/hora en que se ejecutan.
/// Por defecto: LUNES 7 de enero de 2030, 08:00.
/// Los servicios deben usar TimeProvider (inyectado) en lugar de DateTime.Now.
/// </summary>
public sealed class RelojFijo : TimeProvider
{
    public static readonly DateTime LunesBase = new(2030, 1, 7, 8, 0, 0);

    public DateTime Ahora { get; set; }

    public RelojFijo() : this(LunesBase) { }

    public RelojFijo(DateTime ahora) => Ahora = ahora;

    public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(Ahora, DateTimeKind.Utc));

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
