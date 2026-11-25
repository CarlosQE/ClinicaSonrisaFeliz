using ClinicaDental.Contrato.Respuestas.Especialidad;
using ClinicaDental.Contrato.Respuestas.HorarioOdontologo;

namespace ClinicaDental.Contrato.Respuestas.Odontologo
{
    public class OdontologoRespuesta
    {
        public Guid Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid IdEspecialidad { get; set; }
        public bool Activo { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public EspecialidadRespuesta? Especialidad { get; set; }
        public List<HorarioRespuesta> Horarios { get; set; } = new();
    }
}
