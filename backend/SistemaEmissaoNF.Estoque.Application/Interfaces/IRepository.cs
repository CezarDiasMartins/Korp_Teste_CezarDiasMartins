using System.Linq.Expressions;

namespace SistemaEmissaoNF.Estoque.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    Task<List<T>> ListAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task InsertAsync(T entity, CancellationToken cancellationToken);

    void Update(T entity);

    Task SaveAsync(CancellationToken cancellationToken);
}
