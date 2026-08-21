namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands;

public abstract class ProdutoCommandBase
{
    public long Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}