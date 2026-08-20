using MediatR;
using SistemaEmissaoNF.Estoque.Application.Response;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;

public class UpdateProdutoCommand : IRequest<GenericDataResponse<ProdutoResponse>>
{
    public int Id { get; set; }

    public long Codigo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Saldo { get; set; }
}
