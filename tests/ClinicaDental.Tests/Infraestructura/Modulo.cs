namespace ClinicaDental.Tests.Infraestructura;

/// <summary>
/// Nombres de módulo para las categorías de prueba. Cada clase de test lleva
/// [Trait("Modulo", Modulo.X)] para poder correr solo lo de un grupo:
///
///   dotnet test --filter "Modulo=Citas"
///   dotnet test --filter "Modulo=Pacientes"
///   dotnet test --filter "Modulo=Base"        (infraestructura común, debe estar SIEMPRE en verde)
/// </summary>
public static class Modulo
{
    public const string Base = "Base";
    public const string Pacientes = "Pacientes";       // ApostolesEvoMorales
    public const string Odontologos = "Odontologos";   // BagayerosChaco
    public const string Citas = "Citas";               // AndresFundamentalistas
    public const string Tratamientos = "Tratamientos"; // Ultimo
    public const string Pagos = "Pagos";               // GuaposMisteriosos
}
