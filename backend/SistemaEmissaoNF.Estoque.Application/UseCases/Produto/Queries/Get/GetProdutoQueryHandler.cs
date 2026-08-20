using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.Get;

public class GetProdutoQueryHandler(IProdutoRepository produtoRepository, IMapper mapper)
    : IRequestHandler<GetProdutoQuery, GenericDataResponse<ProdutoResponse>>
{
    public async Task<GenericDataResponse<ProdutoResponse>> Handle(GetProdutoQuery request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<ProdutoResponse>();
        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (produto is null)
        {
            response.Errors.Add("Produto nao encontrado.");
            return response;
        }

        response.Data = mapper.Map<ProdutoResponse>(produto);
        return response;
    }
}
