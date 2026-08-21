namespace SistemaEmissaoNF.Faturamento.Application.Response;

public class GenericDataResponse<T> : IResponse
{
    public bool Success => Errors.Count == 0;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
}
