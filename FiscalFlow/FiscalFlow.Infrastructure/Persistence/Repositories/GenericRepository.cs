using System.Linq.Expressions;
using FiscalFlow.Domain.Core.Primitives;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    #region Fields

    protected readonly AppDbContext _context;

    #endregion

    #region Constructors

    protected GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    #endregion

    #region Methods

    public void Add(TEntity objModel)
    {
        _context.Set<TEntity>().Add(objModel);
        _context.SaveChanges();
    }

    public void AddRange(IEnumerable<TEntity> objModel)
    {
        _context.Set<TEntity>().AddRange(objModel);
        _context.SaveChanges();
    }

    public bool Any(Func<TEntity, bool> predicate)
    {
        return _context.Set<TEntity>().Any(predicate);
    }

    public TEntity? GetById(Guid id)
    {
        return _context.Set<TEntity>().Find(id);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().FirstOrDefault(predicate);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().Where(predicate).ToList();
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public IQueryable<TEntity> GetAllAsQuery()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public async Task<IQueryable<TEntity>> GetAllAsQueryAsync()
    {
        return await Task.Run(() => _context.Set<TEntity>().AsQueryable());
    }

    public int Count()
    {
        return _context.Set<TEntity>().Count();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<TEntity>().CountAsync();
    }

    public void Update(TEntity objModel)
    {
        _context.Entry(objModel).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Remove(TEntity objModel)
    {
        _context.Set<TEntity>().Remove(objModel);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #endregion
}