namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;

public class CreateNotaFiscalItemCommand
{
    public int ProdutoId { get; set; }
    public long ProdutoCodigo { get; set; }
    public string ProdutoDescricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
