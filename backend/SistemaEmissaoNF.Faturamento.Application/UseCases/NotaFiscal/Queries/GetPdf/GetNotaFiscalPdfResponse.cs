using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Queries.GetPdf;

public class GetNotaFiscalPdfResponse
{
    public StatusImpressao StatusImpressao { get; set; }
    public byte[]? PdfArquivo { get; set; }
    public string Message { get; set; } = string.Empty;
}
