using ClinicaDental.Web.Data;
using ClinicaDental.Web.Models;
using ClinicaDental.Web.Services.Seguridad;

using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories;

public static class UsuarioFactories
{
    public static void Seed(BaseDatos context)
    {
        if (context.usuarios.Any()) return;

        // Usuario único del catálogo: admin / Admin.Sonrisa2026 (hash generado con HasherContrasena).
        var administrador = new Usuario
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
            NombreUsuario = "admin",
            NombreCompleto = "Administrador del sistema",
            ContrasenaHash = HasherContrasena.Hashear("Admin.Sonrisa2026"),
            Rol = Roles.Administrador,
            Activo = true,
            IntentosFallidos = 0,
            BloqueadoHasta = null,
            FechaRegistro = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Unspecified),
            UltimoAcceso = null
        };

        context.usuarios.Add(administrador);
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}
