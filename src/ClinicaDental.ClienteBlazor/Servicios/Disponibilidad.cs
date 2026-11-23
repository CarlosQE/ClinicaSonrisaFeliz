using ClinicaDental.Contrato.Respuestas;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>
    /// Calcula los horarios libres de un odontólogo en un día a partir de sus franjas de trabajo
    /// y de su agenda (la API no expone ese cálculo; aquí solo se sugiere, la API valida al guardar).
    /// </summary>
    public static class Disponibilidad
    {
        public static List<TimeOnly> HorariosLibres(DateOnly fecha, int duracionMinutos, DateTime ahora,
            IEnumerable<HorarioRespuesta> horarios, IEnumerable<CitaRespuesta> citasDelDia, Guid? excluirCita = null)
        {
            var ocupadas = citasDelDia.Where(c => c.Estado != Contrato.Enums.EstadoCita.Cancelada && c.Id != excluirCita).ToList();
            var libres = new List<TimeOnly>();
            var horaActual = DateOnly.FromDateTime(ahora) == fecha ? TimeOnly.FromDateTime(ahora) : TimeOnly.MinValue;
            var esHoy = DateOnly.FromDateTime(ahora) == fecha;

            foreach (var franja in horarios.Where(h => h.DiaSemana == fecha.DayOfWeek).OrderBy(h => h.HoraInicio))
            {
                for (var t = franja.HoraInicio; t.AddMinutes(duracionMinutos) <= franja.HoraFin; t = t.AddMinutes(30))
                {
                    var fin = t.AddMinutes(duracionMinutos);
                    if (esHoy && t <= horaActual) continue;
                    if (ocupadas.Any(c => t < c.HoraFin && c.Hora < fin)) continue;
                    libres.Add(t);
                }
            }
            return libres;
        }
    }
}
