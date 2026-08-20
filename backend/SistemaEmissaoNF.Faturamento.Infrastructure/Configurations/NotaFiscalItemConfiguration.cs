using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEmissaoNF.Faturamento.Domain.Entities;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Configurations;

public class NotaFiscalItemConfiguration : IEntityTypeConfiguration<NotaFiscalItem>
{
    public void Configure(EntityTypeBuilder<NotaFiscalItem> builder)
    {
        builder.ToTable("nota_fiscal_item");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.NotaFiscalId)
            .HasColumnName("nota_fiscal_id")
            .IsRequired();

        builder.Property(x => x.ProdutoId)
            .HasColumnName("produto_id")
            .IsRequired();

        builder.Property(x => x.ProdutoCodigo)
            .HasColumnName("produto_codigo")
            .IsRequired();

        builder.Property(x => x.ProdutoDescricao)
            .HasColumnName("produto_descricao")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Quantidade)
            .HasColumnName("quantidade")
            .IsRequired();

        builder.HasIndex(x => new { x.NotaFiscalId, x.ProdutoId })
            .IsUnique();
    }
}
