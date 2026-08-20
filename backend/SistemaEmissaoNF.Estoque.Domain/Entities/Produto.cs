namespace SistemaEmissaoNF.Estoque.Domain.Entities;

public class Produto
{
    public int Id { get; set; }

    public long Codigo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public int Saldo { get; set; }
}
