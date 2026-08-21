using MediatR;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Mapper;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Response;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Domain.Enums;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;

public class CreateNotaFiscalCommand : IRequest<GenericDataResponse<NotaFiscalResponse>>
{
    public List<CreateNotaFiscalItemCommand> Itens { get; set; } = [];
}

public class CreateNotaFiscalCommandHandler(INotaFiscalRepository notaFiscalRepository) : IRequestHandler<CreateNotaFiscalCommand, GenericDataResponse<NotaFiscalResponse>>
{
    public async Task<GenericDataResponse<NotaFiscalResponse>> Handle(CreateNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        var response = new GenericDataResponse<NotaFiscalResponse>();

        if (request.Itens.GroupBy(x => x.ProdutoId).Any(x => x.Count() > 1))
        {
            response.Errors.Add("Não é permitido informar o mesmo produto mais de uma vez.");
            return response;
        }

        var notaFiscal = new Domain.Entities.NotaFiscal
        {
            NumeroSequencial = await notaFiscalRepository.GetNextNumeroSequencialAsync(cancellationToken),
            Status = StatusNotaFiscal.Aberta,
            StatusImpressao = StatusImpressao.Pendente,
            Itens = request.Itens.Select(x => new NotaFiscalItem
            {
                ProdutoId = x.ProdutoId,
                ProdutoCodigo = x.ProdutoCodigo,
                ProdutoDescricao = x.ProdutoDescricao.Trim(),
                Quantidade = x.Quantidade
            }).ToList()
        };

        await notaFiscalRepository.InsertAsync(notaFiscal, cancellationToken);
        await notaFiscalRepository.SaveAsync(cancellationToken);

        response.Data = notaFiscal.ToResponse();
        return response;
    }
}