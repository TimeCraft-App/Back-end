using TimeCraft.Domain.Enums;

namespace TimeCraft.Core.Services.TimeTracker
{
    public interface ITimeTrackerService
    {
        Task IncreaseUsedDays(int employeeId, int quantity);

        bool TimeoffPossible(int employeeId, TimeoffType timeoffType, int timeoffDays);
    }
}
