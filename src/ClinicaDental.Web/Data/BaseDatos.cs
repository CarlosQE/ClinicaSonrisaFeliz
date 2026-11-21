using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Data
{
    public class BaseDatos:DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options):base(options) { }
        public DbSet<Paciente> pacientes { get; set; }
        public DbSet<Especialidad> especialidades { get; set; }
        public DbSet<Odontologo> odontologos { get; set; }
        public DbSet<HorarioOdontologo> horariosOdontologo { get; set; }
        public DbSet<Cita> citas { get; set; }
        public DbSet<Tratamiento> tratamientos { get; set; }
        public DbSet<Pago> pagos { get; set; }
        public DbSet<Usuario> usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server no permite varias rutas de borrado en cascada, así que ninguna FK borra en cascada
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            DatosIniciales.Sembrar(modelBuilder);
        }
    }
}
