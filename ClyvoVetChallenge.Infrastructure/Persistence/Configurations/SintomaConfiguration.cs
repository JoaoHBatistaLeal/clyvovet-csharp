using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class SintomaConfiguration : IEntityTypeConfiguration<Sintoma>
{
    public void Configure(EntityTypeBuilder<Sintoma> builder)
    {
        builder.ToTable("TB_SINTOMA_CATALOGO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
        
        builder.HasIndex(x => x.Nome).IsUnique();
    }
}