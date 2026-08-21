using FluentValidation;

namespace SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;

public class UpdateProdutoCommandValidator : AbstractValidator<UpdateProdutoCommand>
{
    public UpdateProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Informe um produto válido.");

        RuleFor(x => x.Codigo)
            .GreaterThan(0)
            .WithMessage("Informe um código válido.");

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage("Informe a descrição do produto.");

        RuleFor(x => x.Saldo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O saldo não pode ser negativo.");
    }
}
