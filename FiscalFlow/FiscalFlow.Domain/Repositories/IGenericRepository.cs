using System.Linq.Expressions;
using FiscalFlow.Domain.Core.Primitives;

namespace FiscalFlow.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    public Guid Add(TEntity objModel);

    public void AddRange(IEnumerable<TEntity> objModel);

    bool Any(Func<TEntity, bool> predicate);

    public TEntity? GetById(Guid id);

    public Task<TEntity?> GetByIdAsync(Guid id);

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate);

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

    public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);

    public Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

    public IEnumerable<TEntity> GetAll();

    public Task<IEnumerable<TEntity>> GetAllAsync();

    public IQueryable<TEntity> GetAllAsQuery();

    public Task<IQueryable<TEntity>> GetAllAsQueryAsync();

    public int Count();

    public Task<int> CountAsync();

    public void Update(TEntity objModel);

    public void Remove(TEntity objModel);

    public void Dispose();
}