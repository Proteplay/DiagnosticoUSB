using DiagnosticoUSB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiagnosticoUSB.Data;

public class DiagnosticoContext : DbContext {
    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
    public DbSet<PlanDiagnostico> Planes => Set<PlanDiagnostico>();
    public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();
    public DbSet<Error> Errores => Set<Error>();

    private readonly string _dbPath;

    public DiagnosticoContext(string? dbPath = null) {
        _dbPath = dbPath ?? Path.Combine(AppContext.BaseDirectory, "diagnostico.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");

    protected override void OnModelCreating(ModelBuilder b) {
        // Plan de ejemplo para tener datos desde el principio
        b.Entity<PlanDiagnostico>().HasData(
            new PlanDiagnostico { Id = 1, Nombre = "Rápido", Descripcion = "Chequeo básico de RAM y disco", Nivel = 1 }
        );

        // Borrado en cascada, como indica tu modelo relacional (B:C)
        b.Entity<Diagnostico>().HasMany(d => d.Errores).WithOne(e => e.Diagnostico)
            .HasForeignKey(e => e.DiagnosticoId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Dispositivo>().HasMany(d => d.Diagnosticos).WithOne(x => x.Dispositivo)
            .HasForeignKey(x => x.DispositivoId).OnDelete(DeleteBehavior.Cascade);
    }
}
