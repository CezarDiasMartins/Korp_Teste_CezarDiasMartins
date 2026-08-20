using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.List;

public class ListNotaFiscalQuery : IRequest<ListPagedResponse<NotaFiscalResponse>>
{
    public int Page { get; set; } = 1;

    public int QuantityData { get; set; } = 10;
}
