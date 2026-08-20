using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.GetPdf;

public class GetNotaFiscalPdfQueryHandler(INotaFiscalRepository notaFiscalRepository)
    : IRequestHandler<GetNotaFiscalPdfQuery, GenericDataResponse<GetNotaFiscalPdfResponse>>
{
    public async Task<GenericDataResponse<GetNotaFiscalPdfResponse>> Handle(GetNotaFiscalPdfQuery request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<GetNotaFiscalPdfResponse>();
        var notaFiscal = await notaFiscalRepository.GetWithItensAsync(request.Id, cancellationToken);

        if (notaFiscal is null)
        {
            response.Errors.Add("Nota fiscal nao encontrada.");
            return response;
        }

        response.Data = new GetNotaFiscalPdfResponse
        {
            StatusImpressao = notaFiscal.StatusImpressao,
            PdfArquivo = notaFiscal.PdfArquivo,
            Message = notaFiscal.StatusImpressao switch
            {
                StatusImpressao.Concluido when notaFiscal.PdfArquivo is not null => "PDF gerado com sucesso.",
                StatusImpressao.Erro => "Falha ao gerar o PDF da nota fiscal.",
                _ => "PDF ainda esta sendo gerado."
            }
        };

        return response;
    }
}
