using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Domain.Dtos.EmployeeDtos
{
    public class EmployeeCreateDto
    {
        public string UserId { get; set; }

        public int? SalaryId { get; set; }

        public int PositionId { get; set; }
    }
}
