namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public interface INotaFiscalPdfService
{
    Task ProcessarPdfAsync(int notaFiscalId, CancellationToken cancellationToken);
}
