using FluentValidation;
using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Estoque.Commands.Baixar;

public class BaixarEstoqueCommandHandler(
    IProdutoRepository produtoRepository,
    IValidator<BaixarEstoqueCommand> validator)
    : IRequestHandler<BaixarEstoqueCommand, GenericNoDataResponse>
{
    public async Task<GenericNoDataResponse> Handle(BaixarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericNoDataResponse();
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            response.Errors.AddRange(validation.Errors.Select(x => x.ErrorMessage));
            return response;
        }

        if (request.Itens.GroupBy(x => x.ProdutoId).Any(x => x.Count() > 1))
        {
            response.Errors.Add("Nao e permitido baixar o mesmo produto mais de uma vez na mesma nota fiscal.");
            return response;
        }

        response.Errors.AddRange(await produtoRepository.BaixarEstoqueAsync(request.Itens, cancellationToken));
        return response;
    }
}
