using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class Salary : BaseEntity
    {
        public double GrossSalary { get; set; }

        public double NetoSalary { get; set; }

        public string ContractType { get; set; }

        public int PositionId { get; set; }
        public Position? Position { get; set; }
    }
}
