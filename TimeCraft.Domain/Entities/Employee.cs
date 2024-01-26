using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public int? UserId { get; set; }
        
        public User? User { get; set; }

        public int? SalaryId { get; set; }

        public Salary? Salary { get; set; }

        public int PositionId { get; set; }

        public Position? Position { get; set; }
    
        public virtual List<Project> Projects { get; set; } = new List<Project>();
        public virtual List<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

        public virtual List<TimeoffRequest> TimeoffRequests { get; set; } = new List<TimeoffRequest>();

        public virtual List<TimeWorked> TimeWorked { get; set; } = new List<TimeWorked>();

    }
}
