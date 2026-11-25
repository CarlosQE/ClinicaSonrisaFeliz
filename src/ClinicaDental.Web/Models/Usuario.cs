using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.Models
{
    /// <summary>Usuario del sistema (personal de la clínica). La contraseña NUNCA se guarda en texto plano.</summary>
    [Index(nameof(NombreUsuario), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>Hash PBKDF2-SHA256 con sal aleatoria (ver Services/Seguridad/HasherContrasena.cs).</summary>
        [Required, MaxLength(200)]
        public string ContrasenaHash { get; set; } = string.Empty;

        /// <summary>Uno de los valores de <see cref="Roles"/>.</summary>
        [Required, MaxLength(30)]
        public string Rol { get; set; } = Roles.Recepcionista;

        public bool Activo { get; set; } = true;

        /// <summary>Intentos fallidos consecutivos; al llegar al máximo la cuenta se bloquea temporalmente.</summary>
        public int IntentosFallidos { get; set; }

        public DateTime? BloqueadoHasta { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? UltimoAcceso { get; set; }
    }

}
