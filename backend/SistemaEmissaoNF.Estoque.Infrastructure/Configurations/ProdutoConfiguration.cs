using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEmissaoNF.Estoque.Domain.Entities;

namespace SistemaEmissaoNF.Estoque.Infrastructure.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produto");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Codigo)
            .HasColumnName("codigo")
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Saldo)
            .HasColumnName("saldo")
            .IsRequired();

        builder.HasIndex(x => x.Codigo)
            .IsUnique();
    }
}
