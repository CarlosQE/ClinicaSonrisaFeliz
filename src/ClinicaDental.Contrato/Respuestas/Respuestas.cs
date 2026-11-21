using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.Contrato.Respuestas
{
    // Forma del JSON que DEVUELVE la API (las entidades de ClinicaDental.Web.Models serializadas).
    // Si un grupo agrega una propiedad en el backend, agrégala aquí para verla en el cliente.

    public class PacienteRespuesta
    {
        public Guid Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
    }

    public class EspecialidadRespuesta
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class OdontologoRespuesta
    {
        public Guid Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool Activo { get; set; }
        public Guid IdEspecialidad { get; set; }
        public EspecialidadRespuesta? Especialidad { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
    }

    public class HorarioRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdOdontologo { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }

    public class CitaRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        public PacienteRespuesta? Paciente { get; set; }
        public Guid IdOdontologo { get; set; }
        public OdontologoRespuesta? Odontologo { get; set; }
        public DateTime FechaHora { get; set; }
        public int DuracionMinutos { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; }
        public string? MotivoCancelacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public bool EstaVigente { get; set; }
    }

    public class TratamientoRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdPaciente { get; set; }
        public PacienteRespuesta? Paciente { get; set; }
        public Guid IdOdontologo { get; set; }
        public OdontologoRespuesta? Odontologo { get; set; }
        public Guid? IdCita { get; set; }
        public CitaRespuesta? Cita { get; set; }
        public TipoTratamiento Tipo { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Costo { get; set; }
        public string? Observaciones { get; set; }
    }

    public class PagoRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdTratamiento { get; set; }
        public TratamientoRespuesta? Tratamiento { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public MetodoPago Metodo { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public string? Observacion { get; set; }
    }

    /// <summary>GET /api/Paciente/{id}/historial</summary>
    public class HistorialPacienteRespuesta
    {
        public PacienteRespuesta Paciente { get; set; } = new();
        public List<CitaRespuesta> Citas { get; set; } = new();
        public List<TratamientoRespuesta> Tratamientos { get; set; } = new();
    }

    /// <summary>GET /api/Pago/deudas</summary>
    public class DeudaRespuesta
    {
        public Guid IdTratamiento { get; set; }
        public Guid IdPaciente { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCI { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public decimal Costo { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal Saldo { get; set; }
    }

    /// <summary>GET /api/Pago/tratamiento/{id}/saldo</summary>
    public class SaldoRespuesta
    {
        public Guid IdTratamiento { get; set; }
        public decimal SaldoPendiente { get; set; }
    }

    /// <summary>GET /api/Auth/usuarios y GET /api/Auth/yo</summary>
    public class UsuarioRespuesta
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }

    /// <summary>GET /api/Pago/{id}/comprobante</summary>
    public class ComprobanteRespuesta
    {
        public Guid IdPago { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCI { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public DateOnly FechaTratamiento { get; set; }
        public decimal CostoTratamiento { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public decimal SaldoRestante { get; set; }
    }
}
