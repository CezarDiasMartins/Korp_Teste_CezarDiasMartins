using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.GetPdf;

public class GetNotaFiscalPdfQuery : IRequest<GenericDataResponse<GetNotaFiscalPdfResponse>>
{
    public int Id { get; set; }
}
