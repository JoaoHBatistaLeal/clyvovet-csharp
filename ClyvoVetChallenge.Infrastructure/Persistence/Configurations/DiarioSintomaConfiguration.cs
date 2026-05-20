using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class DiarioSintomaConfiguration : IEntityTypeConfiguration<DiarioSintoma>
{
    public void Configure(EntityTypeBuilder<DiarioSintoma> builder)
    {
        builder.ToTable("TB_DIARIO_SINTOMA");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Intensidade).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.Diario)
            .WithMany(d => d.SintomasRegistrados)
            .HasForeignKey(x => x.DiarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Sintoma)
            .WithMany()
            .HasForeignKey(x => x.SintomaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}