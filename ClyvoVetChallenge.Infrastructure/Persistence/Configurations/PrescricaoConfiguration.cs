using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class PrescricaoConfiguration : IEntityTypeConfiguration<Prescricao>
{
    public void Configure(EntityTypeBuilder<Prescricao> builder)
    {
        builder.ToTable("TB_PRESCRICAO");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Medicamento).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Dosagem).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Frequencia).IsRequired().HasMaxLength(50);
        builder.Property(x => x.DuracaoDias).IsRequired();
        builder.Property(x => x.Observacao).HasMaxLength(500);
        builder.Property(x => x.DataPrescricao).IsRequired();

        builder.HasOne<Consulta>()
            .WithMany()
            .HasForeignKey(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}