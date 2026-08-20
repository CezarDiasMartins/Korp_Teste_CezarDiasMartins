namespace SistemaEmissaoNF.Faturamento.Domain.Entities;

public class NotaFiscalItem
{
    public int Id { get; set; }

    public int NotaFiscalId { get; set; }

    public int ProdutoId { get; set; }

    public long ProdutoCodigo { get; set; }

    public string ProdutoDescricao { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public virtual NotaFiscal? NotaFiscal { get; set; }
}
