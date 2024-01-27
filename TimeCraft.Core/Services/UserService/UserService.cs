using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.UserService
{
    public class UserService : IUserService, ICrudOperations<User>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<int> Create(User entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
