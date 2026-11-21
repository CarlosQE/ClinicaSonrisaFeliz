using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Factories
{
    public static class UsuarioFactories
    {
        public static readonly Guid Administrador = Guid.Parse("70000000-0000-0000-0000-000000000001");

        /// <summary>
        /// Usuario inicial: <c>admin</c> / <c>Admin.Sonrisa2026</c> (hash PBKDF2 precalculado; la contraseña no se guarda).
        /// Cámbiala desde el cliente web ("Mi cuenta") después del primer ingreso.
        /// </summary>
        public static void Sembrar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = Administrador,
                    NombreUsuario = "admin",
                    NombreCompleto = "Administrador del sistema",
                    ContrasenaHash = "PBKDF2$100000$K4v0wtZSoSBERCBGlB7fhQ==$eTwO89QHlPU+KfGKeEJ1AfLBQOmxtKmSps1har/sJ6A=",
                    Rol = Roles.Administrador,
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 11, 1)
                }
            );
        }
    }
}
