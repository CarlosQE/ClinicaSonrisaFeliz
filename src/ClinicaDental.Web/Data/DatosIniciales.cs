using ClinicaDental.Web.Factories;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Data
{
    public static class DatosIniciales
    {
        public static void Sembrar(ModelBuilder modelBuilder)
        {
            // Módulo 2 - Odontólogos, especialidades y horarios (BagayerosChaco)
            EspecialidadFactories.Sembrar(modelBuilder);
            OdontologoFactories.Sembrar(modelBuilder);
            HorarioOdontologoFactories.Sembrar(modelBuilder);

            // Módulo 1 - Pacientes (ApostolesEvoMorales)
            PacienteFactories.Sembrar(modelBuilder);

            // Módulo 4 - Tratamientos (Ultimo)
            TratamientoFactories.Sembrar(modelBuilder);

            // Módulo 5 - Pagos (GuaposMisteriosos)
            PagoFactories.Sembrar(modelBuilder);

            // Seguridad - usuario administrador inicial (grupo líder)
            UsuarioFactories.Sembrar(modelBuilder);
        }
    }
}
