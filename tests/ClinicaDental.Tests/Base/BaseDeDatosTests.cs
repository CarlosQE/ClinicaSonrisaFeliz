using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Tests.Base;

/// <summary>
/// Reglas que garantiza la propia base de datos (índices únicos, claves foráneas) y los datos de ejemplo.
/// Deben estar SIEMPRE en verde: si fallan, se rompió el modelo compartido (ClinicaDbContext / entidades).
/// </summary>
[Trait("Modulo", Modulo.Base)]
public class BaseDeDatosTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();
    private readonly Escenario _e;

    public BaseDeDatosTests() => _e = _bd.Escenario();

    public void Dispose() => _bd.Dispose();

    [Fact]
    public void RelojDePrueba_EmpiezaUnLunesALas8()
    {
        Assert.Equal(DayOfWeek.Monday, RelojFijo.LunesBase.DayOfWeek);
        Assert.Equal(8, new RelojFijo().GetLocalNow().Hour);
    }

    [Fact]
    public async Task CI_DePaciente_EsUnico()
    {
        await _e.PacienteAsync(ci: "111");
        await Assert.ThrowsAsync<DbUpdateException>(() => _e.PacienteAsync(ci: "111"));
    }

    [Fact]
    public async Task Matricula_DeOdontologo_EsUnica()
    {
        await _e.OdontologoAsync(matricula: "M-1");
        await Assert.ThrowsAsync<DbUpdateException>(() => _e.OdontologoAsync(matricula: "M-1"));
    }

    [Fact]
    public async Task NumeroComprobante_EsUnico()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id);
        await _e.PagoAsync(t.Id, 10m, "CMP-000001");
        await Assert.ThrowsAsync<DbUpdateException>(() => _e.PagoAsync(t.Id, 10m, "CMP-000001"));
    }

    [Fact]
    public async Task NoSePuedeBorrarPacienteConCitas_PorClaveForanea()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        await _e.CitaAsync(p.Id, o.Id, _e.ProximoLunesA(9));

        using var db = _bd.NuevoContexto();
        db.Pacientes.Remove(await db.Pacientes.SingleAsync());
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task BorrarOdontologo_BorraSusHorarios()
    {
        var o = await _e.OdontologoAsync();
        using var db = _bd.NuevoContexto();
        db.Odontologos.Remove(await db.Odontologos.SingleAsync(x => x.Id == o.Id));
        await db.SaveChangesAsync();
        Assert.Empty(db.HorariosOdontologo);
    }

    [Fact]
    public async Task Decimales_SePuedenSumarYCompararEnLaConsulta()
    {
        var (p, o) = await _e.PacienteYOdontologoAsync();
        var t = await _e.TratamientoAsync(p.Id, o.Id, costo: 100.50m);
        await _e.PagoAsync(t.Id, 20.25m);
        await _e.PagoAsync(t.Id, 30.25m);

        using var db = _bd.NuevoContexto();
        var suma = await db.Pagos.Where(x => x.TratamientoId == t.Id).SumAsync(x => x.Monto);
        var caros = await db.Tratamientos.CountAsync(x => x.Costo > 100m);

        Assert.Equal(50.50m, suma);
        Assert.Equal(1, caros);
    }

    [Fact]
    public void DatosIniciales_CarganTodoYNoDuplicanAlRepetir()
    {
        using var db = _bd.NuevoContexto();
        DatosIniciales.Sembrar(db, _bd.Reloj);
        DatosIniciales.Sembrar(db, _bd.Reloj);

        Assert.Equal(4, db.Especialidades.Count());
        Assert.Equal(4, db.Odontologos.Count());
        Assert.Equal(5, db.Pacientes.Count());
        Assert.True(db.Citas.Count() >= 5);
        Assert.Equal(1, db.Tratamientos.Count());
        Assert.Equal(1, db.Pagos.Count());
        Assert.Contains(db.Odontologos, o => !o.Activo);
        Assert.Contains(db.Citas, c => c.Estado == EstadoCita.Programada && c.FechaHora > _bd.Reloj.Ahora);
    }
}
