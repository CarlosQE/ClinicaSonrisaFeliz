namespace ClinicaDental.Contrato.Enums
{
    // Mismos valores enteros que ClinicaDental.Web.Models (la API los envía y recibe como int).

    public enum EstadoCita
    {
        Programada = 0,
        Reprogramada = 1,
        Cancelada = 2,
        Atendida = 3
    }
}
