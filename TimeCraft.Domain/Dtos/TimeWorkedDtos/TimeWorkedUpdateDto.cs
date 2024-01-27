using TimeCraft.Domain.Entities;

namespace TimeCraft.Domain.Dtos.TimeWorkedDtos
{
    public class TimeWorkedUpdateDto
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Duration { get; set; }

        public int ProjectId { get; set; }

        public int ProjectTaskId { get; set; }

        public string Description { get; set; }
    }
}
