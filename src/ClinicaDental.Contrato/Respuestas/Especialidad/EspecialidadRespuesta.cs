namespace ClinicaDental.Contrato.Respuestas.Especialidad
{
    public class EspecialidadRespuesta
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
