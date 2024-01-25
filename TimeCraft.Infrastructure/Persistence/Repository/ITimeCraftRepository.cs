using System.Linq.Expressions;

namespace TimeCraft.Infrastructure.Persistence.Repository
{
    public interface ITimeCraftRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression, bool asNoTracking = false, params string[] includes);
        IQueryable<TEntity> GetByConditionPaginated(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> orderBy, int page, int pageSize, bool orderByDescending = true);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetById(Expression<Func<TEntity, bool>> expression);

        void Create(TEntity entity);
        void CreateRange(List<TEntity> entity);
        void CreateRangeList(List<List<TEntity>> entities);
        void Delete(TEntity entity);
        void DeleteRange(List<TEntity> entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entity);
        int Count(Expression<Func<TEntity, bool>> expression);
    }
}
