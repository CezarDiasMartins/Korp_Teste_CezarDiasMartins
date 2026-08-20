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
