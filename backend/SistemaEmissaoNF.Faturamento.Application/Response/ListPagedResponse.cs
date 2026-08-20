namespace SistemaEmissaoNF.Faturamento.Application.Response;

public class ListPagedResponse<T>
{
    public bool Success => Errors.Count == 0;

    public List<T> Data { get; set; } = [];

    public int Page { get; set; }

    public int QuantityData { get; set; }

    public int TotalData { get; set; }

    public int TotalPage { get; set; }

    public bool Previous => Page > 1;

    public bool Next => Page < TotalPage;

    public List<string> Errors { get; set; } = [];
}
