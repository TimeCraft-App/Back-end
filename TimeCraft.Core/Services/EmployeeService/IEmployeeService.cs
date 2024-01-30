using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;

namespace TimeCraft.Core.Services.EmployeeService
{
    public interface IEmployeeService<T> : ICrudOperations<T> where T : class
    {
        IQueryable<Employee> SearchEmployees(int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize);
    }
}
