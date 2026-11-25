using ClinicaDental.Web.Factories;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Data
{
    public static class DatosIniciales
    {
        public static void Sembrar(BaseDatos context, int cantidadExtra)
        {
            // Una sola transacción: si falla cualquier lote, no quedan tablas a medias.
            using var transaccion = context.Database.BeginTransaction();

            // El orden importa por las llaves foráneas.
            EspecialidadFactories.Seed(context, cantidadExtra);
            OdontologoFactories.Seed(context, cantidadExtra);
            HorarioOdontologoFactories.Seed(context, cantidadExtra);
            PacienteFactories.Seed(context, cantidadExtra);
            CitaFactories.Seed(context, cantidadExtra);
            TratamientoFactories.Seed(context, cantidadExtra);
            PagoFactories.Seed(context, cantidadExtra);
            UsuarioFactories.Seed(context);

            transaccion.Commit();
        }
    }
}
