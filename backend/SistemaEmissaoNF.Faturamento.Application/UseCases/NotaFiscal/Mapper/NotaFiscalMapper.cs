using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Mapper;

public static class NotaFiscalMapper
{
    public static NotaFiscalResponse ToResponse(this Domain.Entities.NotaFiscal notaFiscal)
    {
        return new NotaFiscalResponse
        {
            Id = notaFiscal.Id,
            NumeroSequencial = notaFiscal.NumeroSequencial,
            Status = ((char)notaFiscal.Status).ToString(),
            StatusDescricao = notaFiscal.Status == StatusNotaFiscal.Aberta ? "Aberta" : "Fechada",
            StatusImpressao = ((char)notaFiscal.StatusImpressao).ToString(),
            StatusImpressaoDescricao = notaFiscal.StatusImpressao switch
            {
                StatusImpressao.Pendente => "Pendente",
                StatusImpressao.Gerando => "Gerando",
                StatusImpressao.Concluido => "Concluída",
                StatusImpressao.Erro => "Erro",
                _ => notaFiscal.StatusImpressao.ToString()
            },
            Itens = notaFiscal.Itens.Select(x => new NotaFiscalItemResponse
            {
                Id = x.Id,
                ProdutoId = x.ProdutoId,
                ProdutoCodigo = x.ProdutoCodigo,
                ProdutoDescricao = x.ProdutoDescricao,
                Quantidade = x.Quantidade
            }).ToList()
        };
    }
}
