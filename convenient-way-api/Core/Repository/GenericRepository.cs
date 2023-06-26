using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.Context;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;
using System.Linq.Expressions;

namespace ship_convenient.Core.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected AppDbContext _context;
        protected DbSet<TEntity> _dbSet;
        protected readonly ILogger _logger;
        public GenericRepository(AppDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual TEntity? GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }
        public virtual void Insert(IList<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual bool Delete(Guid id)
        {
            TEntity? entity = _dbSet.Find(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entites)
        {
            _dbSet.RemoveRange(entites);
        }

        public virtual void DeleteRange(IList<TEntity> entites)
        {
            _dbSet.RemoveRange(entites);
        }

        public virtual TEntity Update(TEntity entity)
        {
            return _dbSet.Update(entity).Entity;
        }

        public virtual void Update(IEnumerable<TEntity> entites)
        {
            _dbSet.UpdateRange(entites);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking) query.AsNoTracking();
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            return (await _dbSet.AddAsync(entity)).Entity;
        }


        public virtual async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        public virtual async Task InsertAsync(IList<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            TEntity? entity = await _dbSet.FindAsync(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }

        public virtual IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
          bool disableTracking = true, bool ignoreQueqyFilter = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueqyFilter)
            {
                query = query.IgnoreQueryFilters();
            }
            if (orderBy != null)
            {
                return orderBy(query).Select(selector);
            }
            else
            {
                return query.Select(selector);
            }
        }

        public virtual async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
              Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
              bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<List<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector, List<Expression<Func<TEntity, bool>>>? predicates = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
          bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates != null && predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).ToListAsync();
            }
            else
            {
                return await query.Select(selector).ToListAsync();
            }
        }

        public virtual PaginatedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
          int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.ToPaginatedList<TEntity>(pageIndex, pageSize);
        }

        public virtual Task<PaginatedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.ToPaginatedListAsync<TEntity>(pageIndex, pageSize);
        }

        public virtual Task<PaginatedList<TEntity>> GetPagedListAsync(
            List<Expression<Func<TEntity, bool>>>? predicates = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates != null && predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.ToPaginatedListAsync<TEntity>(pageIndex, pageSize);
        }

        public virtual PaginatedList<TResult> GetPagedList<TResult>(
            Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            bool ignoreQueryFilters = false) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.Select(selector).ToPaginatedList<TResult>(pageIndex, pageSize);
        }

        public virtual Task<PaginatedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default,
            bool ignoreQueryFilters = false) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.Select(selector).ToPaginatedListAsync<TResult>(pageIndex, pageSize);
        }

        public virtual Task<PaginatedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            List<Expression<Func<TEntity, bool>>>? predicates = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default,
            bool ignoreQueryFilters = false) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates != null && predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }
            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.Select(selector).ToPaginatedListAsync<TResult>(pageIndex, pageSize);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true)
        {
            TEntity? result;
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            } 
            result = await query.FirstOrDefaultAsync(predicate);
            return result;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(List<Expression<Func<TEntity, bool>>> predicates,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null, bool disableTracking = true)
        {
            TEntity? result;
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }
            result = await query.FirstOrDefaultAsync();
            return result;
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public async Task<List<TEntity>> GetAllAsync(List<Expression<Func<TEntity, bool>>> predicates,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool disableTracking = true,
            bool ignoreQueqyFilter = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates != null && predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (ignoreQueqyFilter)
            {
                query = query.IgnoreQueryFilters();
            }
            return await query.ToListAsync();
        }

        public List<TEntity> GetAll(List<Expression<Func<TEntity, bool>>> predicates, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool disableTracking = true, bool ignoreQueqyFilter = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicates != null && predicates.Count > 0)
            {
                int count = predicates.Count;
                for (int i = 0; i < count; i++)
                {
                    query = query.Where(predicates[i]);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (ignoreQueqyFilter)
            {
                query = query.IgnoreQueryFilters();
            }
            return query.ToList();
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (disableTracking)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null) {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }
            return query.ToList();
        }
    }
}
