using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Mapper;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.Get;

public class GetNotaFiscalQuery : IRequest<GenericDataResponse<NotaFiscalResponse>>
{
    public int Id { get; set; }
}

public class GetNotaFiscalQueryHandler(INotaFiscalRepository notaFiscalRepository)
    : IRequestHandler<GetNotaFiscalQuery, GenericDataResponse<NotaFiscalResponse>>
{
    public async Task<GenericDataResponse<NotaFiscalResponse>> Handle(GetNotaFiscalQuery request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<NotaFiscalResponse>();
        var notaFiscal = await notaFiscalRepository.GetWithItensAsync(request.Id, cancellationToken);

        if (notaFiscal is null)
        {
            response.Errors.Add("Nota fiscal não encontrada.");
            return response;
        }

        response.Data = notaFiscal.ToResponse();
        return response;
    }
}