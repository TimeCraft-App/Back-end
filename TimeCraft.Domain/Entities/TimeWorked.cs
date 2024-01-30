namespace TimeCraft.Domain.Entities
{
    public class TimeWorked : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
                
        public DateTime WorkDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Duration { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public int ProjectTaskId { get; set; }

        public ProjectTask ProjectTask { get; set; }

        public string Description { get; set; }
    }
}
