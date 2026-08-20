using SistemaEmissaoNF.Faturamento.Application.Messaging;

namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public interface INotaFiscalPdfPublisher
{
    Task PublishAsync(GerarNotaFiscalPdfCommand command, CancellationToken cancellationToken);
}
