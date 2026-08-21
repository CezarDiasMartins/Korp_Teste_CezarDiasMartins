using MediatR;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Create;

public class CreateProdutoCommand : ProdutoCommandBase, IRequest<GenericDataResponse<ProdutoResponse>>;

public class CreateProdutoCommandHandler(IProdutoRepository produtoRepository, IMapper mapper) : IRequestHandler<CreateProdutoCommand, GenericDataResponse<ProdutoResponse>>
{
    public async Task<GenericDataResponse<ProdutoResponse>> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<ProdutoResponse>();

        if (await produtoRepository.CodigoExistsAsync(request.Codigo, null, cancellationToken))
        {
            response.Errors.Add("Já existe um produto cadastrado com este código.");
            return response;
        }

        var produto = new Domain.Entities.Produto
        {
            Codigo = request.Codigo,
            Descricao = request.Descricao.Trim(),
            Saldo = request.Saldo
        };

        await produtoRepository.InsertAsync(produto, cancellationToken);
        await produtoRepository.SaveAsync(cancellationToken);

        response.Data = mapper.Map<ProdutoResponse>(produto);
        return response;
    }
}