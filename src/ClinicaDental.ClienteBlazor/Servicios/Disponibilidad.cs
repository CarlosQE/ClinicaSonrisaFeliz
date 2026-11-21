using ClinicaDental.Contrato.Respuestas;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>
    /// Calcula los horarios libres de un odontólogo en un día a partir de sus franjas de trabajo
    /// y de su agenda (la API no expone ese cálculo; aquí solo se sugiere, la API valida al guardar).
    /// </summary>
    public static class Disponibilidad
    {
        public static List<DateTime> HorariosLibres(DateOnly fecha, int duracionMinutos, DateTime ahora,
            IEnumerable<HorarioRespuesta> horarios, IEnumerable<CitaRespuesta> citasDelDia, Guid? excluirCita = null)
        {
            var ocupadas = citasDelDia.Where(c => c.Estado != Contrato.Enums.EstadoCita.Cancelada && c.Id != excluirCita).ToList();
            var libres = new List<DateTime>();

            foreach (var franja in horarios.Where(h => h.DiaSemana == fecha.DayOfWeek).OrderBy(h => h.HoraInicio))
            {
                var limite = fecha.ToDateTime(franja.HoraFin);
                for (var t = fecha.ToDateTime(franja.HoraInicio); t.AddMinutes(duracionMinutos) <= limite; t = t.AddMinutes(30))
                {
                    var fin = t.AddMinutes(duracionMinutos);
                    if (t <= ahora) continue;
                    if (ocupadas.Any(c => t < c.FechaHoraFin && c.FechaHora < fin)) continue;
                    libres.Add(t);
                }
            }
            return libres;
        }
    }
}
