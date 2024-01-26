using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class TimeoffRequest : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public string Type { get; set; } // TimeoffType enum

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }
    }
}
