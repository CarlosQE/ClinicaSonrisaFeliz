namespace ClinicaDental.Contrato.Respuestas.Paciente
{
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
}
