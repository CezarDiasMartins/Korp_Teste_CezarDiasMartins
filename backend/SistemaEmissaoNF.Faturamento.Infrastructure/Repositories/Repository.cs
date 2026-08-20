using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Infrastructure.Context;

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Repositories;

public class Repository<T>(FaturamentoDbContext dbContext) : IRepository<T> where T : class
{
    protected readonly FaturamentoDbContext DbContext = dbContext;
    protected readonly DbSet<T> DbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<List<T>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
