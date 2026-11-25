namespace ClinicaDental.Contrato.Respuestas.HorarioOdontologo
{
    public class HorarioRespuesta
    {
        public Guid Id { get; set; }
        public Guid IdOdontologo { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }
}
