using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.Get;

public class GetNotaFiscalQuery : IRequest<GenericDataResponse<NotaFiscalResponse>>
{
    public int Id { get; set; }
}
