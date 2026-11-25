using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class HorarioOdontologoFactories
{
    private const int CantidadBase = 40;
    private const int TamanoLote = 200;

    public static void Seed(BaseDatos context, int cantidadExtra)
    {
        if (context.horariosOdontologo.Any()) return;

        context.Database.SetCommandTimeout(120);
        var extras = Math.Clamp(cantidadExtra, 0, 1000 - CantidadBase);
        var lote = new List<HorarioOdontologo>(TamanoLote);

        var odontologosBase = new[]
        {
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            Guid.Parse("20000000-0000-0000-0000-000000000003"),
            Guid.Parse("20000000-0000-0000-0000-000000000004")
        };
        var dias = new[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday
        };
        var turnos = new[]
        {
            (Inicio: new TimeOnly(8, 0), Fin: new TimeOnly(12, 0)),
            (Inicio: new TimeOnly(14, 0), Fin: new TimeOnly(18, 0))
        };

        // Los 40 horarios históricos: lunes a viernes, dos turnos, para los cuatro odontólogos base.
        var indice = 1;
        foreach (var idOdontologo in odontologosBase)
        {
            foreach (var dia in dias)
            {
                foreach (var turno in turnos)
                {
                    lote.Add(new HorarioOdontologo
                    {
                        Id = Guid.Parse($"40000000-0000-0000-0000-{indice:D12}"),
                        IdOdontologo = idOdontologo,
                        DiaSemana = dia,
                        HoraInicio = turno.Inicio,
                        HoraFin = turno.Fin
                    });
                    indice++;
                }
            }
        }

        context.horariosOdontologo.AddRange(lote);
        context.SaveChanges();
        context.ChangeTracker.Clear();
        lote.Clear();

        var odontologosExtras = context.odontologos
            .Where(o => !odontologosBase.Contains(o.Id))
            .OrderBy(o => o.Matricula)
            .Select(o => o.Id)
            .ToList();

        if (odontologosExtras.Count == 0) return;

        // Cada odontólogo extra recorre en orden los 10 espacios posibles (5 días x 2 turnos),
        // sin repetir ninguno: así nunca hay solapes ni duplicados por odontólogo.
        var espaciosPorOdontologo = dias.Length * turnos.Length;
        var total = Math.Min(extras, odontologosExtras.Count * espaciosPorOdontologo);

        for (var i = 0; i < total; i++)
        {
            var ciclo = i / odontologosExtras.Count;
            var turno = turnos[ciclo / dias.Length];
            lote.Add(new HorarioOdontologo
            {
                Id = Guid.NewGuid(),
                IdOdontologo = odontologosExtras[i % odontologosExtras.Count],
                DiaSemana = dias[ciclo % dias.Length],
                HoraInicio = turno.Inicio,
                HoraFin = turno.Fin
            });

            if (lote.Count == TamanoLote)
            {
                context.horariosOdontologo.AddRange(lote);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            context.horariosOdontologo.AddRange(lote);
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
    }
}