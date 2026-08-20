namespace SistemaEmissaoNF.Faturamento.Application.Messaging;

public class GerarNotaFiscalPdfCommand
{
    public Guid CorrelationId { get; set; }

    public int NotaFiscalId { get; set; }

    public long NumeroSequencial { get; set; }
}
