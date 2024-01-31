using TimeCraft.Core.Services.LeaveManager;
using TimeCraft.Core.Services.TimeoffBalanceService;
using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;

namespace TimeCraft.Core.Services.TimeTracker
{
    public class TimeTrackerService : ITimeTrackerService
    {
        private readonly ITimeoffBalanceService<TimeoffBalance> _timeoffBalanceService;
        private readonly ILeaveManagerService _leaveManagerService;

        private int TOTAL_USED_DAYS { get; set; } = -1;

        public TimeTrackerService(ITimeoffBalanceService<TimeoffBalance> timeoffBalanceService, ILeaveManagerService leaveManagerService)
        {
            _timeoffBalanceService = timeoffBalanceService;
            _leaveManagerService = leaveManagerService;
        }

        public async Task IncreaseUsedDays(int employeeId, int quantity)
        {
            var totalUsedDays = await _timeoffBalanceService.CalculateUsedDays(employeeId);

            TOTAL_USED_DAYS = totalUsedDays + quantity;
        }

        public bool TimeoffPossible(int employeeId, TimeoffType timeoffType, int timeoffDays)
        {
            var employeeBalance = _timeoffBalanceService.SearchTimeoffBalances(employeeId).FirstOrDefault();

            return !_leaveManagerService.ExceedsBalance(timeoffType, employeeBalance, timeoffDays);
        }
    }
}
