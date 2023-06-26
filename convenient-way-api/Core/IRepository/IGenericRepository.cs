using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using System.Linq.Expressions;

namespace ship_convenient.Core.IRepository
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        IEnumerable<TEntity> GetAll();
        TEntity? GetById(Guid id);
        TEntity Insert(TEntity entity);
        void Insert(IEnumerable<TEntity> entities);
        void Insert(IList<TEntity> entities);
        bool Delete(Guid id);
        void DeleteRange(IEnumerable<TEntity> entities);
        void DeleteRange(IList<TEntity> entities);
        TEntity Update(TEntity entity);
        void Update(IEnumerable<TEntity> entites);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<TEntity?> GetByIdAsync(Guid id, Func<IQueryable<TEntity>,
            IIncludableQueryable<TEntity, object?>>? include = null, bool disableTracking = true);
        Task<TEntity> InsertAsync(TEntity entity);
        Task InsertAsync(IEnumerable<TEntity> entities);
        Task InsertAsync(IList<TEntity> entities);
        int Count();
        Task<bool> DeleteAsync(Guid id);
        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true);
        Task<TEntity?> FirstOrDefaultAsync(
            List<Expression<Func<TEntity, bool>>> predicates,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true);
        PaginatedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20,
            bool disableTracking = true, bool ignoreQueryFilters = false);

        PaginatedList<TResult> GetPagedList<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            bool ignoreQueryFilters = false) where TResult : class;

        Task<PaginatedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false);

        Task<PaginatedList<TEntity>> GetPagedListAsync(
            List<Expression<Func<TEntity, bool>>>? predicates = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false);

        Task<PaginatedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false) where TResult : class;
        Task<PaginatedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            List<Expression<Func<TEntity, bool>>>? predicates = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            int pageIndex = 0, int pageSize = 20, bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken),
            bool ignoreQueryFilters = false) where TResult : class;

        IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true,
            bool ignoreQueqyFilter = false);
        Task<List<TEntity>> GetAllAsync(List<Expression<Func<TEntity, bool>>> predicates,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool disableTracking = true,
            bool ignoreQueqyFilter = false);

        Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilter = false);

        Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            List<Expression<Func<TEntity, bool>>>? predicates = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);
        List<TEntity> GetAll(List<Expression<Func<TEntity, bool>>> predicates,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
           bool disableTracking = true,
           bool ignoreQueqyFilter = false);
        List<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
            bool disableTracking = true,
            bool ignoreQueryFilter = false);
    }
}
