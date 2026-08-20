using FluentValidation;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;

public class UpdateProdutoCommandValidator : AbstractValidator<UpdateProdutoCommand>
{
    public UpdateProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Informe um produto valido.");

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
