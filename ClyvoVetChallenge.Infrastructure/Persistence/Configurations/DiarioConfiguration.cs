using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class DiarioConfiguration : IEntityTypeConfiguration<Diario>
{
    public void Configure(EntityTypeBuilder<Diario> builder)
    {
        builder.ToTable("TB_DIARIO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataRegistro).IsRequired();
        builder.Property(x => x.HoraRegistro).IsRequired();
        builder.Property(x => x.ObservacaoLivre).HasMaxLength(1000);

        builder.HasOne(x => x.Animal)
            .WithMany()
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}