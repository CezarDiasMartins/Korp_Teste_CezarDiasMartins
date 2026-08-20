using MediatR;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Create;

public class CreateProdutoCommand : IRequest<GenericDataResponse<ProdutoResponse>>
{
    public long Codigo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Saldo { get; set; }
}
