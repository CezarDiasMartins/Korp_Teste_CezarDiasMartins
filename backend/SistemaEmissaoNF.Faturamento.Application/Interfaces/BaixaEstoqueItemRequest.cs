namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public class BaixaEstoqueItemRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
