namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Response;

public class ProdutoResponse
{
    public int Id { get; set; }

    public long Codigo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Saldo { get; set; }
}
