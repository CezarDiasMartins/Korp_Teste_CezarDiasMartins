using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Infrastructure.Configurations;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Context;

public class FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : DbContext(options)
{
    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();

    public DbSet<NotaFiscalItem> NotaFiscalItens => Set<NotaFiscalItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<long>("nota_fiscal_numero_seq")
            .StartsAt(1)
            .IncrementsBy(1);

        modelBuilder.ApplyConfiguration(new NotaFiscalConfiguration());
        modelBuilder.ApplyConfiguration(new NotaFiscalItemConfiguration());
    }
}
