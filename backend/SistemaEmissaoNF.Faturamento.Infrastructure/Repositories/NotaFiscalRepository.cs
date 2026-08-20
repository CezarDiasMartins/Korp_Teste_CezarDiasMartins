using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Infrastructure.Context;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Repositories;

public class NotaFiscalRepository(FaturamentoDbContext dbContext)
    : Repository<NotaFiscal>(dbContext), INotaFiscalRepository
{
    public async Task<long> GetNextNumeroSequencialAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Database
            .SqlQueryRaw<long>("SELECT nextval('nota_fiscal_numero_seq')")
            .SingleAsync(cancellationToken);
    }

    public async Task<NotaFiscal?> GetWithItensAsync(int id, CancellationToken cancellationToken)
    {
        return await DbContext.NotasFiscais
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<NotaFiscal>> ListWithItensAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await DbContext.NotasFiscais
            .AsNoTracking()
            .Include(x => x.Itens)
            .OrderByDescending(x => x.NumeroSequencial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
