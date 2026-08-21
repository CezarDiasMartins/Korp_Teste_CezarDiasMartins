namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

public class NotaFiscalItemResponse
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public long ProdutoCodigo { get; set; }
    public string ProdutoDescricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
