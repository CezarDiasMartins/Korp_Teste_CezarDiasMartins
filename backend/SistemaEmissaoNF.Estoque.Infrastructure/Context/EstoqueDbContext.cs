using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Estoque.Domain.Entities;
using SistemaEmissaoNF.Estoque.Infrastructure.Configurations;

namespace SistemaEmissaoNF.Estoque.Infrastructure.Context;

public class EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : DbContext(options)
{
    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
    }
}
