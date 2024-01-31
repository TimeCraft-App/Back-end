using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Domain.Dtos.TimeWorkedDtos
{
    public class TimeWorkedCreateDto
    {
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Duration { get; set; }

        public int ProjectId { get; set; }

        public int ProjectTaskId { get; set; }

        public string Description { get; set; }
    }
}
