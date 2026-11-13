using ClinicaDental.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaDental.Web.Data;

/// <summary>
/// ÚNICO DbContext del sistema. Dueño: grupo líder.
/// Si tu módulo necesita una tabla o columna nueva, pídelo por Pull Request
/// tocando SOLO tu entidad y (si hace falta) tu bloque de configuración aquí abajo.
/// </summary>
public class ClinicaDbContext : DbContext
{
    public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : base(options) { }

    // Módulo 1 - Pacientes
    public DbSet<Paciente> Pacientes => Set<Paciente>();

    // Módulo 2 - Odontólogos
    public DbSet<Odontologo> Odontologos => Set<Odontologo>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<HorarioOdontologo> HorariosOdontologo => Set<HorarioOdontologo>();

    // Módulo 3 - Citas
    public DbSet<Cita> Citas => Set<Cita>();

    // Módulo 4 - Tratamientos
    public DbSet<Tratamiento> Tratamientos => Set<Tratamiento>();

    // Módulo 5 - Pagos
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite no tiene tipo decimal nativo: sin esto no se puede hacer Sum/OrderBy/comparar
        // decimales dentro de la consulta. Se guarda como REAL (double). Suficiente para montos en Bs.
        configurationBuilder.Properties<decimal>().HaveConversion<double>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------------- Módulo 1 - Pacientes ----------------
        modelBuilder.Entity<Paciente>(e =>
        {
            e.HasIndex(p => p.CI).IsUnique();
        });

        // ---------------- Módulo 2 - Odontólogos ----------------
        modelBuilder.Entity<Especialidad>(e =>
        {
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        modelBuilder.Entity<Odontologo>(e =>
        {
            e.HasIndex(o => o.Matricula).IsUnique();
            e.HasOne(o => o.Especialidad)
             .WithMany(s => s.Odontologos)
             .HasForeignKey(o => o.EspecialidadId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HorarioOdontologo>(e =>
        {
            e.HasOne(h => h.Odontologo)
             .WithMany(o => o.Horarios)
             .HasForeignKey(h => h.OdontologoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------------- Módulo 3 - Citas ----------------
        modelBuilder.Entity<Cita>(e =>
        {
            e.HasOne(c => c.Paciente)
             .WithMany(p => p.Citas)
             .HasForeignKey(c => c.PacienteId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.Odontologo)
             .WithMany(o => o.Citas)
             .HasForeignKey(c => c.OdontologoId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(c => c.Estado).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(c => new { c.OdontologoId, c.FechaHora });
            e.HasIndex(c => new { c.PacienteId, c.FechaHora });
        });

        // ---------------- Módulo 4 - Tratamientos ----------------
        modelBuilder.Entity<Tratamiento>(e =>
        {
            e.HasOne(t => t.Paciente)
             .WithMany(p => p.Tratamientos)
             .HasForeignKey(t => t.PacienteId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Odontologo)
             .WithMany()
             .HasForeignKey(t => t.OdontologoId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Cita)
             .WithMany(c => c.Tratamientos)
             .HasForeignKey(t => t.CitaId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(t => t.Tipo).HasConversion<string>().HasMaxLength(30);
        });

        // ---------------- Módulo 5 - Pagos ----------------
        modelBuilder.Entity<Pago>(e =>
        {
            e.HasIndex(p => p.NumeroComprobante).IsUnique();
            e.HasOne(p => p.Tratamiento)
             .WithMany(t => t.Pagos)
             .HasForeignKey(p => p.TratamientoId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(p => p.Metodo).HasConversion<string>().HasMaxLength(20);
        });
    }
}
