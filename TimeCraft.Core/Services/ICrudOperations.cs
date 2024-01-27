using TimeCraft.Core.Constants;

namespace TimeCraft.Core.Services
{
    internal interface ICrudOperations<T>
    {
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = CommonConstants.DefaultPageSize);

        Task<IEnumerable<T>> GetAsync(int id); 

        Task<int> Create(T entity);

        Task Delete(int id);
    }
}
