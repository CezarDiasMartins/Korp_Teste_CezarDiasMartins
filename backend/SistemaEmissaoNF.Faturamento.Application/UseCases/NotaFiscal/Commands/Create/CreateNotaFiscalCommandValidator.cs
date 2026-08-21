using FluentValidation;

namespace SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;

public class CreateNotaFiscalCommandValidator : AbstractValidator<CreateNotaFiscalCommand>
{
    public CreateNotaFiscalCommandValidator()
    {
        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("Informe ao menos um item para a nota fiscal.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(x => x.ProdutoId)
                .GreaterThan(0)
                .WithMessage("Informe um produto válido.");

            item.RuleFor(x => x.ProdutoCodigo)
                .GreaterThan(0)
                .WithMessage("Informe o código do produto.");

            item.RuleFor(x => x.ProdutoDescricao)
                .NotEmpty()
                .WithMessage("Informe a descrição do produto.");

            item.RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("Informe uma quantidade válida.");
        });
    }
}
