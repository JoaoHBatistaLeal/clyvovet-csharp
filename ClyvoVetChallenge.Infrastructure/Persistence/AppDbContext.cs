global using ClyvoVetContext = ClyvoVetChallenge.Infrastructure.Persistence.AppDbContext;
using Microsoft.EntityFrameworkCore;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Animal> Animais { get; set; }
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<Diario> Diarios { get; set; }
    public DbSet<DiarioSintoma> DiarioSintomas { get; set; }
    public DbSet<Dono> Donos { get; set; }
    public DbSet<Historico> Historicos { get; set; }
    public DbSet<Prescricao> Prescricoes { get; set; }
    public DbSet<Sintoma> Sintomas { get; set; }
    public DbSet<Veterinario> Veterinarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        base.ConfigureConventions(configurationBuilder);
    }
}

public class UtcDateTimeConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}
