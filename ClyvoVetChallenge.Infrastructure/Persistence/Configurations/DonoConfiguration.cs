using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class DonoConfiguration : IEntityTypeConfiguration<Dono>
{
    public void Configure(EntityTypeBuilder<Dono> builder)
    {
        builder.ToTable("TB_DONO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Cpf).IsRequired().HasMaxLength(14);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Telefone).HasMaxLength(20);
        builder.Property(x => x.Endereco).HasMaxLength(255);

        builder.HasIndex(x => x.Cpf).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}