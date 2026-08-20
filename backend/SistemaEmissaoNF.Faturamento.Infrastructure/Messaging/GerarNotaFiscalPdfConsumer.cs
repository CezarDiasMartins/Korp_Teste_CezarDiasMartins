using MassTransit;
using Microsoft.Extensions.Logging;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Messaging;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Messaging;

public class GerarNotaFiscalPdfConsumer(
    INotaFiscalPdfService notaFiscalPdfService,
    ILogger<GerarNotaFiscalPdfConsumer> logger)
    : IConsumer<GerarNotaFiscalPdfCommand>
{
    public async Task Consume(ConsumeContext<GerarNotaFiscalPdfCommand> context)
    {
        logger.LogInformation(
            "Gerando PDF da NF {NotaFiscalId}. CorrelationId: {CorrelationId}",
            context.Message.NotaFiscalId,
            context.Message.CorrelationId);

        await notaFiscalPdfService.ProcessarPdfAsync(context.Message.NotaFiscalId, context.CancellationToken);
    }
}
