using System.Linq.Expressions;
using FiscalFlow.Domain.Core.Primitives;

namespace FiscalFlow.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    void Add(TEntity objModel);
    void AddRange(IEnumerable<TEntity> objModel);

    TEntity? GetById(Guid id);
    Task<TEntity?> GetByIdAsync(Guid id);

    TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

    IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync();

    int Count();
    Task<int> CountAsync();

    void Update(TEntity objModel);
    void Remove(TEntity objModel);

    void Dispose();
}