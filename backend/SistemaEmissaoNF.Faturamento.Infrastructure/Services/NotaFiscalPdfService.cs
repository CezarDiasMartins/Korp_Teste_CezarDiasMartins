using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Services;

public class NotaFiscalPdfService(
    INotaFiscalRepository notaFiscalRepository,
    ILogger<NotaFiscalPdfService> logger)
    : INotaFiscalPdfService
{
    public async Task ProcessarPdfAsync(int notaFiscalId, CancellationToken cancellationToken)
    {
        var notaFiscal = await notaFiscalRepository.GetWithItensAsync(notaFiscalId, cancellationToken);
        if (notaFiscal is null)
        {
            logger.LogWarning("Nota fiscal {NotaFiscalId} não encontrada para geração de PDF.", notaFiscalId);
            return;
        }

        try
        {
            notaFiscal.StatusImpressao = StatusImpressao.Gerando;
            notaFiscalRepository.Update(notaFiscal);
            await notaFiscalRepository.SaveAsync(cancellationToken);

            QuestPDF.Settings.License = LicenseType.Community;
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.Content().Column(column =>
                    {
                        column.Spacing(12);
                        column.Item().Text("NOTA FISCAL").Bold().FontSize(20);
                        column.Item().Text($"Número: {notaFiscal.NumeroSequencial:D6}");
                        column.Item().Text("Status: Fechada");
                        column.Item().LineHorizontal(1);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(5);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Código").Bold();
                                header.Cell().Text("Descrição").Bold();
                                header.Cell().Text("Quantidade").Bold();
                            });

                            foreach (var item in notaFiscal.Itens)
                            {
                                table.Cell().Text(item.ProdutoCodigo.ToString());
                                table.Cell().Text(item.ProdutoDescricao);
                                table.Cell().Text(item.Quantidade.ToString());
                            }
                        });
                    });
                });
            }).GeneratePdf();

            notaFiscal.PdfArquivo = pdf;
            notaFiscal.PdfGeradoEm = DateTime.UtcNow;
            notaFiscal.StatusImpressao = StatusImpressao.Concluido;
            notaFiscalRepository.Update(notaFiscal);
            await notaFiscalRepository.SaveAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao gerar PDF da NF {NotaFiscalId}.", notaFiscalId);
            notaFiscal.StatusImpressao = StatusImpressao.Erro;
            notaFiscalRepository.Update(notaFiscal);
            await notaFiscalRepository.SaveAsync(cancellationToken);
        }
    }
}
