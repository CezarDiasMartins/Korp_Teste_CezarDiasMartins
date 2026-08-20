using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Domain.Entities;

public class NotaFiscal
{
    public int Id { get; set; }

    public long NumeroSequencial { get; set; }

    public StatusNotaFiscal Status { get; set; } = StatusNotaFiscal.Aberta;

    public StatusImpressao StatusImpressao { get; set; } = StatusImpressao.Pendente;

    public byte[]? PdfArquivo { get; set; }

    public DateTime? PdfGeradoEm { get; set; }

    public virtual List<NotaFiscalItem> Itens { get; set; } = [];
}
