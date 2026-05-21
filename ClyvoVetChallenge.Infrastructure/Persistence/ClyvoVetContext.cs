using Microsoft.EntityFrameworkCore;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence;

public class ClyvoVetContext(DbContextOptions<ClyvoVetContext> options) : DbContext(options)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClyvoVetContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}