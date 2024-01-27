namespace TimeCraft.Domain.Dtos.TimeWorkedDtos
{
    public class TimeWorkedCreateDto
    {
        public DateTime WorkDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Duration { get; set; }

        public int ProjectId { get; set; }

        public int ProjectTaskId { get; set; }

        public string Description { get; set; }
    }
}
