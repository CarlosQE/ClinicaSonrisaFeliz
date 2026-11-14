using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class PagoFactories
    {
        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pago>().HasData(
                new Pago { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), IdTratamiento = TratamientoFactories.Trat2, Monto = 300.00m, FechaPago = new DateTime(2026, 11, 6, 12, 0, 0), Metodo = MetodoPago.Efectivo, NumeroComprobante = "CMP-000001", Observacion = "Adelanto" }
            );
        }
    }
}
