namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;

public class NotaFiscalResponse
{
    public int Id { get; set; }

    public long NumeroSequencial { get; set; }

    public string NumeroFormatado => NumeroSequencial.ToString("D6");

    public string Status { get; set; } = string.Empty;

    public string StatusDescricao { get; set; } = string.Empty;

    public string StatusImpressao { get; set; } = string.Empty;

    public string StatusImpressaoDescricao { get; set; } = string.Empty;

    public List<NotaFiscalItemResponse> Itens { get; set; } = [];
}
