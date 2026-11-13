namespace ClinicaDental.Web.Models;

// ==========================================================================
//  CONTRATO COMPARTIDO (dueño: grupo líder). NO renombrar ni borrar valores.
//  Se pueden AGREGAR valores nuevos al final si un grupo lo necesita (avisar).
// ==========================================================================

/// <summary>Ciclo de vida de una cita (módulo 3 - Citas).</summary>
public enum EstadoCita
{
    Programada = 0,
    Reprogramada = 1,
    Cancelada = 2,
    Atendida = 3
}

/// <summary>Tipos de tratamiento que ofrece la clínica (módulo 4 - Tratamientos).</summary>
public enum TipoTratamiento
{
    LimpiezaDental = 0,
    Ortodoncia = 1,
    Endodoncia = 2,
    Extraccion = 3
}

/// <summary>Formas de pago aceptadas (módulo 5 - Pagos).</summary>
public enum MetodoPago
{
    Efectivo = 0,
    Tarjeta = 1,
    Transferencia = 2,
    QR = 3
}
