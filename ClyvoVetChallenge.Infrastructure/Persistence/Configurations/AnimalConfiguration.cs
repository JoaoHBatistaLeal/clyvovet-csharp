using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.ToTable("TB_ANIMAL");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Especie).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Raca).HasMaxLength(50);
        builder.Property(x => x.Sexo).HasMaxLength(20);
        builder.Property(x => x.DataNascimento).IsRequired();
        builder.Property(x => x.Peso).HasColumnType("decimal(5,2)").IsRequired();

        builder.HasOne(x => x.Dono)
            .WithMany(x => x.Animais)
            .HasForeignKey(x => x.DonoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}