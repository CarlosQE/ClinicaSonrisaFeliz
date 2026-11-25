namespace ClinicaDental.Contrato.Respuestas.Auth
{
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
}
