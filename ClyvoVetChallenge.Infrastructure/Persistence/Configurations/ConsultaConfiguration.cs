using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Enums;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("TB_CONSULTA");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataConsulta).IsRequired();
        builder.Property(x => x.HoraConsulta).IsRequired();
        builder.Property(x => x.Motivo).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Diagnostico).HasMaxLength(1000);
        builder.Property(x => x.Observacoes).HasMaxLength(1000);
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Tipo).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.Animal)
            .WithMany()
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veterinario)
            .WithMany()
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Clinica)
            .WithMany()
            .HasForeignKey(x => x.ClinicaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}