using TimeCraft.Domain.Dtos.LeaveManagerDtos;

namespace TimeCraft.Core.Services.LeaveManager
{
    public interface ILeaveManagerService
    {
        Task<bool> ApplyForTimeoffRequest(string userId, TimeoffRequestApplicationDto data);

        Task ApproveTimeoffRequest(int id);

        Task RejectTimeoffRequest(int id);
    }
}
