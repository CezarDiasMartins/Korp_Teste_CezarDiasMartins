using MediatR;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.List;

public class ListProdutoQuery : IRequest<ListPagedResponse<ProdutoResponse>>
{
    public int Page { get; set; } = 1;

    public int QuantityData { get; set; } = 10;
}
