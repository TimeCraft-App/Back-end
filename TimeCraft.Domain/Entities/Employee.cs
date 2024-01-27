namespace TimeCraft.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string UserId { get; set; }
        
        public User User { get; set; }

        public int? SalaryId { get; set; }

        public Salary Salary { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }
    
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

        public virtual ICollection<TimeoffRequest> TimeoffRequests { get; set; } = new List<TimeoffRequest>();

        public virtual ICollection<TimeWorked> TimeWorked { get; set; } = new List<TimeWorked>();
    }
}
