namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public class BaixaEstoqueRequest
{
    public int NotaFiscalId { get; set; }

    public Guid CorrelationId { get; set; }

    public List<BaixaEstoqueItemRequest> Itens { get; set; } = [];
}
