using TimeCraft.Infrastructure.Persistence.Repository;

namespace TimeCraft.Infrastructure.Persistence.UnitOfWork
{
    public interface IUnitOfWork
    {
        public ITimeCraftRepository<TEntity> Repository<TEntity>() where TEntity : class;
        bool Complete();
        Task<bool> CompleteAsync();
    }
}
