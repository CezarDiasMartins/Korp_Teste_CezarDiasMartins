namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public interface IEstoqueService
{
    Task<BaixaEstoqueResponse> BaixarEstoqueAsync(BaixaEstoqueRequest request, CancellationToken cancellationToken);
}
