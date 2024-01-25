using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeCraft.Infrastructure.Persistence.Data;

namespace TimeCraft.Infrastructure.Persistence.Repository
{
    public class TimeCraftRepository<TEntity> : ITimeCraftRepository<TEntity> where TEntity : class
    {
        private readonly DataContext _dbContext;

        public TimeCraftRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression, bool asNoTracking = false, params string[] includes)
        {
            var queryable = _dbContext.Set<TEntity>().Where(expression);
            foreach (var item in includes)
            {
                queryable = queryable.Include(item);
            }

            if (asNoTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            return queryable;
        }

        public IQueryable<TEntity> GetByConditionPaginated(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            const int defaultPageNumber = 1;

            var query = _dbContext.Set<TEntity>().Where(expression);

            // Check if the page number is greater then zero - otherwise use default page number
            if (page <= 0)
            {
                page = defaultPageNumber;
            }

            if (orderBy != null)
            {
                query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<TEntity> GetAll()
        {
            var result = _dbContext.Set<TEntity>();

            return result;
        }

        public IQueryable<TEntity> GetById(Expression<Func<TEntity, bool>> expression)
        {
            return _dbContext.Set<TEntity>().Where(expression);
        }

        public void Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void CreateRange(List<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
        }

        public void CreateRangeList(List<List<TEntity>> entities)
        {
            foreach (List<TEntity> entityList in entities)
            {
                _dbContext.Set<TEntity>().AddRange(entityList);
            }
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange(List<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
        }

        public int Count(Expression<Func<TEntity, bool>> expression)
        {
            return _dbContext.Set<TEntity>().Count(expression);
        }
    }
}
