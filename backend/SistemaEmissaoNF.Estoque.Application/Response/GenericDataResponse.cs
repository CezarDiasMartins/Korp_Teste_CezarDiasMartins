namespace SistemaEmissaoNF.Estoque.Application.Response;

public class GenericDataResponse<T>
{
    public bool Success => Errors.Count == 0;

    public T? Data { get; set; }

    public List<string> Errors { get; set; } = [];
}
