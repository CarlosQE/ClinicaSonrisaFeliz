using System.Reflection;
using ClinicaDental.Tests.Infraestructura;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Contratos;

namespace ClinicaDental.Tests.Base;

/// <summary>
/// Protege los CONTRATOS compartidos. Si este test falla es porque alguien cambió una firma
/// de una interfaz o renombró/borró una propiedad de una entidad: eso rompe el trabajo de otro grupo.
/// Cualquier cambio aquí debe acordarse con el líder y actualizarse en el README.
/// (Agregar propiedades NUEVAS a una entidad sí está permitido y no rompe este test.)
/// </summary>
[Trait("Modulo", Modulo.Base)]
public class ContratosTests
{
    public static TheoryData<Type, string[]> FirmasEsperadas => new()
    {
        {
            typeof(IPacienteService), new[]
            {
                "Task<List<Paciente>> ListarAsync(String busqueda)",
                "Task<Paciente> ObtenerPorIdAsync(Int32 id)",
                "Task<Paciente> RegistrarAsync(Paciente paciente)",
                "Task ActualizarAsync(Paciente paciente)",
                "Task EliminarAsync(Int32 id)",
                "Task<HistorialPaciente> ObtenerHistorialAsync(Int32 pacienteId)",
            }
        },
        {
            typeof(IOdontologoService), new[]
            {
                "Task<List<Odontologo>> ListarAsync(Boolean soloActivos)",
                "Task<Odontologo> ObtenerPorIdAsync(Int32 id)",
                "Task<Odontologo> RegistrarAsync(Odontologo odontologo)",
                "Task ActualizarAsync(Odontologo odontologo)",
                "Task CambiarEstadoAsync(Int32 id, Boolean activo)",
                "Task<List<Especialidad>> ListarEspecialidadesAsync()",
                "Task<Especialidad> RegistrarEspecialidadAsync(Especialidad especialidad)",
                "Task<List<HorarioOdontologo>> ListarHorariosAsync(Int32 odontologoId)",
                "Task<HorarioOdontologo> AgregarHorarioAsync(HorarioOdontologo horario)",
                "Task EliminarHorarioAsync(Int32 horarioId)",
            }
        },
        {
            typeof(ICitaService), new[]
            {
                "Task<List<Cita>> ListarAsync(DateOnly? desde, DateOnly? hasta, Int32? odontologoId, Int32? pacienteId, EstadoCita? estado)",
                "Task<Cita> ObtenerPorIdAsync(Int32 id)",
                "Task<Cita> AgendarAsync(Cita cita)",
                "Task<Cita> ReprogramarAsync(Int32 citaId, DateTime nuevaFechaHora, Int32? nuevoOdontologoId)",
                "Task CancelarAsync(Int32 citaId, String motivo)",
                "Task MarcarAtendidaAsync(Int32 citaId)",
                "Task<List<Cita>> ObtenerAgendaDiariaAsync(DateOnly fecha, Int32? odontologoId, Boolean incluirCanceladas)",
                "Task<List<DateTime>> ObtenerHorariosDisponiblesAsync(Int32 odontologoId, DateOnly fecha, Int32 duracionMinutos)",
            }
        },
        {
            typeof(ITratamientoService), new[]
            {
                "Task<List<Tratamiento>> ListarAsync(Int32? pacienteId, TipoTratamiento? tipo)",
                "Task<Tratamiento> ObtenerPorIdAsync(Int32 id)",
                "Task<Tratamiento> RegistrarAsync(Tratamiento tratamiento)",
                "Task ActualizarObservacionesAsync(Int32 tratamientoId, String observaciones)",
                "Task EliminarAsync(Int32 id)",
            }
        },
        {
            typeof(IPagoService), new[]
            {
                "Task<List<Pago>> ListarAsync(Int32? pacienteId)",
                "Task<Pago> ObtenerPorIdAsync(Int32 id)",
                "Task<Pago> RegistrarAsync(Pago pago)",
                "Task<Decimal> ObtenerSaldoPendienteAsync(Int32 tratamientoId)",
                "Task<List<DeudaPendiente>> ConsultarDeudasAsync(Int32? pacienteId)",
                "Task<ComprobantePago> ObtenerComprobanteAsync(Int32 pagoId)",
            }
        },
    };

    [Theory]
    [MemberData(nameof(FirmasEsperadas))]
    public void Interfaz_MantieneLasFirmasAcordadas(Type interfaz, string[] esperadas)
    {
        var reales = interfaz.GetMethods().Select(Firma).OrderBy(x => x).ToArray();
        Assert.Equal(esperadas.OrderBy(x => x).ToArray(), reales);
    }

    public static TheoryData<Type, string[]> PropiedadesEsperadas => new()
    {
        { typeof(Paciente), new[] { "Id:Int32", "Nombres:String", "Apellidos:String", "CI:String", "FechaNacimiento:DateOnly", "Telefono:String", "Email:String", "Direccion:String", "FechaRegistro:DateTime", "NombreCompleto:String", "Citas:List<Cita>", "Tratamientos:List<Tratamiento>" } },
        { typeof(Especialidad), new[] { "Id:Int32", "Nombre:String", "Descripcion:String", "Odontologos:List<Odontologo>" } },
        { typeof(Odontologo), new[] { "Id:Int32", "Nombres:String", "Apellidos:String", "Matricula:String", "Telefono:String", "Email:String", "Activo:Boolean", "EspecialidadId:Int32", "Especialidad:Especialidad", "Horarios:List<HorarioOdontologo>", "Citas:List<Cita>", "NombreCompleto:String" } },
        { typeof(HorarioOdontologo), new[] { "Id:Int32", "OdontologoId:Int32", "Odontologo:Odontologo", "DiaSemana:DayOfWeek", "HoraInicio:TimeOnly", "HoraFin:TimeOnly" } },
        { typeof(Cita), new[] { "Id:Int32", "PacienteId:Int32", "Paciente:Paciente", "OdontologoId:Int32", "Odontologo:Odontologo", "FechaHora:DateTime", "DuracionMinutos:Int32", "Motivo:String", "Estado:EstadoCita", "MotivoCancelacion:String", "FechaRegistro:DateTime", "FechaHoraFin:DateTime", "EstaVigente:Boolean", "Tratamientos:List<Tratamiento>" } },
        { typeof(Tratamiento), new[] { "Id:Int32", "PacienteId:Int32", "Paciente:Paciente", "OdontologoId:Int32", "Odontologo:Odontologo", "CitaId:Int32?", "Cita:Cita", "Tipo:TipoTratamiento", "Fecha:DateOnly", "Costo:Decimal", "Observaciones:String", "Pagos:List<Pago>" } },
        { typeof(Pago), new[] { "Id:Int32", "TratamientoId:Int32", "Tratamiento:Tratamiento", "Monto:Decimal", "FechaPago:DateTime", "Metodo:MetodoPago", "NumeroComprobante:String", "Observacion:String" } },
    };

    [Theory]
    [MemberData(nameof(PropiedadesEsperadas))]
    public void Entidad_ConservaLasPropiedadesAcordadas(Type entidad, string[] esperadas)
    {
        var reales = entidad.GetProperties().Select(p => $"{p.Name}:{NombreTipo(p.PropertyType)}").ToHashSet();
        var faltantes = esperadas.Where(e => !reales.Contains(e)).ToList();
        Assert.True(faltantes.Count == 0, $"{entidad.Name} perdió o cambió: {string.Join(", ", faltantes)}");
    }

    [Fact]
    public void Enums_ConservanSusValores()
    {
        Assert.Equal(new[] { "Programada", "Reprogramada", "Cancelada", "Atendida" }, Enum.GetNames<EstadoCita>().Take(4));
        Assert.Equal(new[] { "LimpiezaDental", "Ortodoncia", "Endodoncia", "Extraccion" }, Enum.GetNames<TipoTratamiento>().Take(4));
        Assert.Equal(new[] { "Efectivo", "Tarjeta", "Transferencia", "QR" }, Enum.GetNames<MetodoPago>().Take(4));
    }

    private static string Firma(MethodInfo m)
        => $"{NombreTipo(m.ReturnType)} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{NombreTipo(p.ParameterType)} {p.Name}"))})";

    private static string NombreTipo(Type t)
    {
        if (Nullable.GetUnderlyingType(t) is { } subyacente) return NombreTipo(subyacente) + "?";
        if (!t.IsGenericType) return t.Name;
        var nombre = t.Name[..t.Name.IndexOf('`')];
        return $"{nombre}<{string.Join(",", t.GetGenericArguments().Select(NombreTipo))}>";
    }
}
