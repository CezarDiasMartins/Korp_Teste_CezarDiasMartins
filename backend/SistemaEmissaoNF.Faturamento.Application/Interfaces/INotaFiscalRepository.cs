using SistemaEmissaoNF.Faturamento.Domain.Entities;

namespace SistemaEmissaoNF.Faturamento.Application.Interfaces;

public interface INotaFiscalRepository : IRepository<NotaFiscal>
{
    Task<long> GetNextNumeroSequencialAsync(CancellationToken cancellationToken);

    Task<NotaFiscal?> GetWithItensAsync(int id, CancellationToken cancellationToken);

    Task<List<NotaFiscal>> ListWithItensAsync(int page, int pageSize, CancellationToken cancellationToken);
}
