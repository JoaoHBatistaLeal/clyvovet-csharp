using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class HistoricoConfiguration : IEntityTypeConfiguration<Historico>
{
    public void Configure(EntityTypeBuilder<Historico> builder)
    {
        builder.ToTable("TB_HISTORICO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataRegistro).IsRequired();
        builder.Property(x => x.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(x => x.TipoEvento).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Resultado).HasMaxLength(500);

        builder.HasOne<Animal>()
            .WithMany()
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Consulta>()
            .WithMany()
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}