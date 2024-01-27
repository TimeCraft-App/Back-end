using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Dtos
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int? SalaryId { get; set; }

        public int PositionId { get; set; }
    }
}
