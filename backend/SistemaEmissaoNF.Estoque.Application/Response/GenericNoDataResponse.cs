namespace SistemaEmissaoNF.Estoque.Application.Response;

public class GenericNoDataResponse
{
    public bool Success => Errors.Count == 0;

    public List<string> Errors { get; set; } = [];
}
