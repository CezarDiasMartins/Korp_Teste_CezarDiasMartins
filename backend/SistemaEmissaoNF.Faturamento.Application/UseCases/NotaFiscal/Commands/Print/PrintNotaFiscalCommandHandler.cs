using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Messaging;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Print;

public class PrintNotaFiscalCommandHandler(
    INotaFiscalRepository notaFiscalRepository,
    IEstoqueService estoqueService,
    INotaFiscalPdfPublisher pdfPublisher)
    : IRequestHandler<PrintNotaFiscalCommand, PrintNotaFiscalResponse>
{
    public async Task<PrintNotaFiscalResponse> Handle(PrintNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        var response = new PrintNotaFiscalResponse();
        var notaFiscal = await notaFiscalRepository.GetWithItensAsync(request.Id, cancellationToken);

        if (notaFiscal is null)
        {
            response.Errors.Add("Nota fiscal nao encontrada.");
            return response;
        }

        if (notaFiscal.Status == StatusNotaFiscal.Fechada)
        {
            response.Errors.Add("Nota fiscal ja esta fechada.");
            return response;
        }

        var correlationId = Guid.NewGuid();
        var baixaResponse = await estoqueService.BaixarEstoqueAsync(new BaixaEstoqueRequest
        {
            NotaFiscalId = notaFiscal.Id,
            CorrelationId = correlationId,
            Itens = notaFiscal.Itens.Select(x => new BaixaEstoqueItemRequest
            {
                ProdutoId = x.ProdutoId,
                Quantidade = x.Quantidade
            }).ToList()
        }, cancellationToken);

        if (!baixaResponse.Success)
        {
            response.ServiceUnavailable = baixaResponse.ServiceUnavailable;
            response.Errors.AddRange(baixaResponse.Errors);
            return response;
        }

        notaFiscal.Status = StatusNotaFiscal.Fechada;
        notaFiscal.StatusImpressao = StatusImpressao.Pendente;
        notaFiscal.PdfArquivo = null;
        notaFiscal.PdfGeradoEm = null;

        notaFiscalRepository.Update(notaFiscal);
        await notaFiscalRepository.SaveAsync(cancellationToken);

        await pdfPublisher.PublishAsync(new GerarNotaFiscalPdfCommand
        {
            CorrelationId = correlationId,
            NotaFiscalId = notaFiscal.Id,
            NumeroSequencial = notaFiscal.NumeroSequencial
        }, cancellationToken);

        return response;
    }
}
