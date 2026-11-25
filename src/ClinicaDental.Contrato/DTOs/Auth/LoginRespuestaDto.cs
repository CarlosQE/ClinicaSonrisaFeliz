namespace ClinicaDental.Contrato.DTOs.Auth
{
    /// <summary>Respuesta de POST /api/Auth/login.</summary>
    public class LoginRespuestaDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }
}
