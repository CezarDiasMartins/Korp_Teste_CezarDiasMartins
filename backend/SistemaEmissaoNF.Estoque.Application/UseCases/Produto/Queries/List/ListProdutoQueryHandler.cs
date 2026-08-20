using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Queries.List;

public class ListProdutoQueryHandler(IProdutoRepository produtoRepository, IMapper mapper)
    : IRequestHandler<ListProdutoQuery, ListPagedResponse<ProdutoResponse>>
{
    public async Task<ListPagedResponse<ProdutoResponse>> Handle(ListProdutoQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.QuantityData <= 0 ? 10 : Math.Min(request.QuantityData, 100);
        var total = await produtoRepository.CountAsync(cancellationToken);
        var produtos = await produtoRepository.ListAsync(page, pageSize, cancellationToken);

        return new ListPagedResponse<ProdutoResponse>
        {
            Data = produtos.Select(mapper.Map<ProdutoResponse>).ToList(),
            Page = page,
            QuantityData = pageSize,
            TotalData = total,
            TotalPage = (int)Math.Ceiling(total / (double)pageSize)
        };
    }
}
