namespace ClinicaDental.Web.Models
{
    public enum EstadoCita
    {
        Programada = 0,
        Reprogramada = 1,
        Cancelada = 2,
        Atendida = 3
    }

    public enum TipoTratamiento
    {
        LimpiezaDental = 0,
        Ortodoncia = 1,
        Endodoncia = 2,
        Extraccion = 3
    }

    public enum MetodoPago
    {
        Efectivo = 0,
        Tarjeta = 1,
        Transferencia = 2,
        QR = 3
    }
}
