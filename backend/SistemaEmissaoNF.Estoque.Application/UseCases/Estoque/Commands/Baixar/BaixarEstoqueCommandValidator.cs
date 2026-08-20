using FluentValidation;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Estoque.Commands.Baixar;

public class BaixarEstoqueCommandValidator : AbstractValidator<BaixarEstoqueCommand>
{
    public BaixarEstoqueCommandValidator()
    {
        RuleFor(x => x.NotaFiscalId)
            .GreaterThan(0)
            .WithMessage("Informe uma nota fiscal valida.");

        RuleFor(x => x.CorrelationId)
            .NotEmpty()
            .WithMessage("Informe o identificador de correlacao.");

        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("Informe ao menos um item para baixa.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(x => x.ProdutoId)
                .GreaterThan(0)
                .WithMessage("Informe um produto valido.");

            item.RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("Informe uma quantidade valida.");
        });
    }
}
