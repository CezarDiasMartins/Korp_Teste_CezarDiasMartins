using MediatR;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Print;

public class PrintNotaFiscalCommand : IRequest<PrintNotaFiscalResponse>
{
    public int Id { get; set; }
}
