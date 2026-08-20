using SistemaEmissaoNF.Faturamento.Application.Response;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Print;

public class PrintNotaFiscalResponse : GenericNoDataResponse
{
    public bool ServiceUnavailable { get; set; }
}
