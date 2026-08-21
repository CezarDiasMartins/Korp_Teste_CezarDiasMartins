using FluentValidation;
using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;

public class UpdateProdutoCommandHandler(
    IProdutoRepository produtoRepository,
    IMapper mapper,
    IValidator<UpdateProdutoCommand> validator)
    : IRequestHandler<UpdateProdutoCommand, GenericDataResponse<ProdutoResponse>>
{
    public async Task<GenericDataResponse<ProdutoResponse>> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<ProdutoResponse>();
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            response.Errors.AddRange(validation.Errors.Select(x => x.ErrorMessage));
            return response;
        }

        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (produto is null)
        {
            response.Errors.Add("Produto não encontrado.");
            return response;
        }

        if (await produtoRepository.CodigoExistsAsync(request.Codigo, request.Id, cancellationToken))
        {
            response.Errors.Add("Já existe um produto cadastrado com este código.");
            return response;
        }

        produto.Codigo = request.Codigo;
        produto.Descricao = request.Descricao.Trim();
        produto.Saldo = request.Saldo;

        produtoRepository.Update(produto);
        await produtoRepository.SaveAsync(cancellationToken);

        response.Data = mapper.Map<ProdutoResponse>(produto);
        return response;
    }
}
