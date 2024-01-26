using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class ProjectTask : BaseEntity
    {
        public int ProjectId { get; set; }

        public Project? Project { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("EmployeeId")]
        public int AssignedToEmployee {  get; set; }

        public string Status {  get; set; } //task enum

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual List<TimeWorked> TimeWorked { get; set; } = new List<TimeWorked>();

    }
}
