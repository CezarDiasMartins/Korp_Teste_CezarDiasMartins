using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Estoque.Commands.Baixar;

public class BaixarEstoqueCommand : IRequest<GenericNoDataResponse>
{
    public int NotaFiscalId { get; set; }
    public Guid CorrelationId { get; set; }
    public List<BaixaEstoqueItemRequest> Itens { get; set; } = [];
}

public class BaixarEstoqueCommandHandler(IProdutoRepository produtoRepository) : IRequestHandler<BaixarEstoqueCommand, GenericNoDataResponse>
{
    public async Task<GenericNoDataResponse> Handle(BaixarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericNoDataResponse();

        if (request.Itens.GroupBy(x => x.ProdutoId).Any(x => x.Count() > 1))
        {
            response.Errors.Add("Não é permitido baixar o mesmo produto mais de uma vez na mesma nota fiscal.");
            return response;
        }

        response.Errors.AddRange(await produtoRepository.BaixarEstoqueAsync(request.Itens, cancellationToken));
        return response;
    }
}