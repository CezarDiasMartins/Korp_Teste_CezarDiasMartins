using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Domain.Entities;
using SistemaEmissaoNF.Estoque.Infrastructure.Context;

namespace SistemaEmissaoNF.Estoque.Infrastructure.Repositories;

public class ProdutoRepository(EstoqueDbContext dbContext)
    : Repository<Produto>(dbContext), IProdutoRepository
{
    public async Task<bool> CodigoExistsAsync(long codigo, int? ignoreId, CancellationToken cancellationToken)
    {
        return await DbContext.Produtos.AnyAsync(
            x => x.Codigo == codigo && (!ignoreId.HasValue || x.Id != ignoreId.Value),
            cancellationToken);
    }

    public async Task<List<string>> BaixarEstoqueAsync(IReadOnlyCollection<BaixaEstoqueItemRequest> itens, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (var item in itens)
        {
            var produto = await DbContext.Produtos.FirstOrDefaultAsync(x => x.Id == item.ProdutoId, cancellationToken);

            if (produto is null)
            {
                errors.Add($"Produto {item.ProdutoId} não encontrado.");
                break;
            }

            if (produto.Saldo < item.Quantidade)
            {
                errors.Add($"Saldo insuficiente para o produto {produto.Codigo}.");
                break;
            }

            produto.Saldo -= item.Quantidade;
        }

        if (errors.Count > 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            DbContext.ChangeTracker.Clear();
            return errors;
        }

        await DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return errors;
    }
}
