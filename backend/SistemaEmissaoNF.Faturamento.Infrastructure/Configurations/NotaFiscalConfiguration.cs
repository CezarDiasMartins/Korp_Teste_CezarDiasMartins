using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Configurations;

public class NotaFiscalConfiguration : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("nota_fiscal");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.NumeroSequencial)
            .HasColumnName("numero_sequencial")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("char(1)")
            .HasConversion(
                value => ((char)value).ToString(),
                value => (StatusNotaFiscal)value[0])
            .IsRequired();

        builder.Property(x => x.StatusImpressao)
            .HasColumnName("status_impressao")
            .HasColumnType("char(1)")
            .HasConversion(
                value => ((char)value).ToString(),
                value => (StatusImpressao)value[0])
            .IsRequired();

        builder.Property(x => x.PdfArquivo)
            .HasColumnName("pdf_arquivo");

        builder.Property(x => x.PdfGeradoEm)
            .HasColumnName("pdf_gerado_em");

        builder.HasIndex(x => x.NumeroSequencial)
            .IsUnique();

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.NotaFiscal)
            .HasForeignKey(x => x.NotaFiscalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
