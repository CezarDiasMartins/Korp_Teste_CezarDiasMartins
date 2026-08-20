using FluentValidation;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Create;

public class CreateProdutoCommandValidator : AbstractValidator<CreateProdutoCommand>
{
    public CreateProdutoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .GreaterThan(0)
            .WithMessage("Informe um codigo valido.");

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage("Informe a descricao do produto.");

        RuleFor(x => x.Saldo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O saldo nao pode ser negativo.");
    }
}
