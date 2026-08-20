using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;

public class CreateNotaFiscalCommand : IRequest<GenericDataResponse<NotaFiscalResponse>>
{
    public List<CreateNotaFiscalItemCommand> Itens { get; set; } = [];
}
