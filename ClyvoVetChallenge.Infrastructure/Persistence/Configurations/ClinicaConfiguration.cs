using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.ToTable("TB_CLINICA");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Cnpj).IsRequired().HasMaxLength(18);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Telefone).HasMaxLength(20);
        builder.Property(x => x.Endereco).HasMaxLength(255);
        builder.Property(x => x.HorarioFuncionamento).HasMaxLength(100);

        builder.HasIndex(x => x.Cnpj).IsUnique();
    }
}