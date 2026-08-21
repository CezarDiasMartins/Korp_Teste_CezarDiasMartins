using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Mapper;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.List;

public class ListNotaFiscalQuery : IRequest<ListPagedResponse<NotaFiscalResponse>>
{
    public int Page { get; set; } = 1;
    public int QuantityData { get; set; } = 10;
}

public class ListNotaFiscalQueryHandler(INotaFiscalRepository notaFiscalRepository)
    : IRequestHandler<ListNotaFiscalQuery, ListPagedResponse<NotaFiscalResponse>>
{
    public async Task<ListPagedResponse<NotaFiscalResponse>> Handle(ListNotaFiscalQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.QuantityData <= 0 ? 10 : Math.Min(request.QuantityData, 100);
        var total = await notaFiscalRepository.CountAsync(cancellationToken);
        var notas = await notaFiscalRepository.ListWithItensAsync(page, pageSize, cancellationToken);

        return new ListPagedResponse<NotaFiscalResponse>
        {
            Data = notas.Select(x => x.ToResponse()).ToList(),
            Page = page,
            QuantityData = pageSize,
            TotalData = total,
            TotalPage = (int)Math.Ceiling(total / (double)pageSize)
        };
    }
}