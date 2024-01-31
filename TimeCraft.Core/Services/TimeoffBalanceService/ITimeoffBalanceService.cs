using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;
using TimeCraft.Infrastructure.Constants;

namespace TimeCraft.Core.Services.TimeoffBalanceService
{
    public interface ITimeoffBalanceService<T> : ICrudOperations<T> where T : class
    {
        IQueryable<TimeoffBalance> SearchTimeoffBalances(int employeeId, int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize);

        Task ChangeBalance(int employeeId, int quantity, TimeoffType type);

        int GetBalance(int employeeId, TimeoffType type);

        Task<int> CalculateUsedDays(int employeeId);
    }
}
