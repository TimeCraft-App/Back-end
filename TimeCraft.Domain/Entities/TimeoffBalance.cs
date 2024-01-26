using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class TimeoffBalance : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public int VacationDays { get; set; }
        public int SickDays { get; set; }
        public int PersonalDays { get; set; }

        public int OtherTimeOffDays { get; set; }
    }
}
