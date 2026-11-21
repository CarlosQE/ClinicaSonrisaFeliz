using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs
{
    /// <summary>Cuerpo de POST /api/Odontologo y PUT /api/Odontologo/{id}.</summary>
    public class OdontologoDto
    {
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [MaxLength(30, ErrorMessage = "La matrícula no puede tener más de 30 caracteres.")]
        [Display(Name = "Matrícula")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(150, ErrorMessage = "El email no puede tener más de 150 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [Display(Name = "Especialidad")]
        public Guid? IdEspecialidad { get; set; }
    }

    /// <summary>Cuerpo de PUT /api/Odontologo/{id}/estado.</summary>
    public class CambiarEstadoOdontologoDto
    {
        public bool? Activo { get; set; }
    }

    /// <summary>Cuerpo de POST/PUT /api/Especialidad.</summary>
    public class EspecialidadDto
    {
        [Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")]
        [MaxLength(80, ErrorMessage = "El nombre no puede tener más de 80 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }

    /// <summary>Cuerpo de POST /api/HorarioOdontologo/odontologo/{id} y PUT /api/HorarioOdontologo/{id}.</summary>
    public class HorarioOdontologoDto
    {
        [Display(Name = "Día")]
        public DayOfWeek DiaSemana { get; set; } = DayOfWeek.Monday;

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [Display(Name = "Hora de inicio")]
        public TimeOnly? HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [Display(Name = "Hora de fin")]
        public TimeOnly? HoraFin { get; set; }
    }
}
