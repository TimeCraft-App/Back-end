using TimeCraft.Infrastructure.Constants;

namespace TimeCraft.Core.Services
{
    public interface ICrudOperations<T>
    {
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = CommonConstants.DefaultPageSize);

        Task<T> GetById(int id); 

        Task<int> Create(T entityToCreate);

        Task Update(T entityToUpdate);

        Task Delete(int id, bool softDelete = true);
    }
}
