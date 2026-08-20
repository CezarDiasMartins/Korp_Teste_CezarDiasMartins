using MassTransit;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Messaging;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Messaging;

public class NotaFiscalPdfPublisher(IPublishEndpoint publishEndpoint) : INotaFiscalPdfPublisher
{
    public async Task PublishAsync(GerarNotaFiscalPdfCommand command, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(command, cancellationToken);
    }
}
