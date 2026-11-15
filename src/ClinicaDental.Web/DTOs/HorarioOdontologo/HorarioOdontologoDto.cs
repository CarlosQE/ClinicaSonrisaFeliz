namespace ClinicaDental.Web.DTOs.HorarioOdontologo
{
    public class HorarioOdontologoDto
    {
        // DayOfWeek: Domingo = 0, Lunes = 1, ..., Sábado = 6.
        public DayOfWeek DiaSemana { get; set; }

        // Formato JSON: "08:00:00".
        public TimeOnly HoraInicio { get; set; }

        // Formato JSON: "12:00:00".
        public TimeOnly HoraFin { get; set; }
    }
}
