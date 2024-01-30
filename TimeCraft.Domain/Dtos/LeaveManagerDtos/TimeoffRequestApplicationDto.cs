using TimeCraft.Domain.Enums;

namespace TimeCraft.Domain.Dtos.LeaveManagerDtos
{
    public class TimeoffRequestApplicationDto
    {
        public int? EmployeeId { get; set; }

        public string Type { get; set; } // Timeoff Type enum

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = TimeoffRequestStatusType.Pending.ToString();

        public string Comment { get; set; }
    }
}
