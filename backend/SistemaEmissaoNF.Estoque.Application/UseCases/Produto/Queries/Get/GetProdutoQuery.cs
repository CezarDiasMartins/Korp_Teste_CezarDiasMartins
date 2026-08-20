using MediatR;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.Get;

public class GetProdutoQuery : IRequest<GenericDataResponse<ProdutoResponse>>
{
    public int Id { get; set; }
}
