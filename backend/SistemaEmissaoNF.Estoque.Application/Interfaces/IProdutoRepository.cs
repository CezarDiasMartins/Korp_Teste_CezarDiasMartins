using SistemaEmissaoNF.Estoque.Domain.Entities;

namespace SistemaEmissaoNF.Estoque.Application.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<bool> CodigoExistsAsync(long codigo, int? ignoreId, CancellationToken cancellationToken);

    Task<List<string>> BaixarEstoqueAsync(IReadOnlyCollection<BaixaEstoqueItemRequest> itens, CancellationToken cancellationToken);
}
