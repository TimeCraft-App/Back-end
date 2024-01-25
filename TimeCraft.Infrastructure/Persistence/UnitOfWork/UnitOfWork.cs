using System.Collections;
using TimeCraft.Infrastructure.Persistence.Data;
using TimeCraft.Infrastructure.Persistence.Repository;

namespace TimeCraft.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        private Hashtable _repositories;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public bool Complete()
        {
            var numberOfAffectedRows = _context.SaveChanges();
            return numberOfAffectedRows > 0;
        }

        public async Task<bool> CompleteAsync()
        {
            var numberOfAffectedRows = await _context.SaveChangesAsync();
            return numberOfAffectedRows > 0;
        }

        public ITimeCraftRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(TimeCraftRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }
            return (ITimeCraftRepository<TEntity>)_repositories[type];
        }
    }
}
