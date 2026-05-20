using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class VeterinarioConfiguration : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.ToTable("TB_VETERINARIO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Crmv).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Especialidade).HasMaxLength(100);
        builder.Property(x => x.Telefone).HasMaxLength(20);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);

        builder.HasIndex(x => x.Crmv).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}