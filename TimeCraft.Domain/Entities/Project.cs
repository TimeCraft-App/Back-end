using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public string Department { get; set; } // DepartmentType enum

        [ForeignKey("EmployeeId")]
        public int? ManagerId { get; set; }

        public Employee Manager { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

        public virtual ICollection<TimeWorked> TimeWorked { get; set; } = new List<TimeWorked>();

    }
}
