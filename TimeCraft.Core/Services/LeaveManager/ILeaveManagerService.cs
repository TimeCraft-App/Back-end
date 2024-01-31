using TimeCraft.Domain.Dtos.LeaveManagerDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;

namespace TimeCraft.Core.Services.LeaveManager
{
    public interface ILeaveManagerService
    {
        Task<bool> ApplyForTimeoffRequest(string userId, TimeoffRequestApplicationDto data);

        Task ApproveTimeoffRequest(int id);

        Task RejectTimeoffRequest(int id);

        bool ExceedsBalance(TimeoffType timeoffType, TimeoffBalance employeeBalance, int timeoffDays);
    }
}
