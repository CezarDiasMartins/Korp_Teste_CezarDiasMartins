namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public class BaixaEstoqueResponse
{
    public bool Success => Errors.Count == 0 && !ServiceUnavailable;
    public bool ServiceUnavailable { get; set; }
    public List<string> Errors { get; set; } = [];
}
