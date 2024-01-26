using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class Position : BaseEntity
    {
        public string Role {  get; set; }   

        public string Description { get; set; }
    
        public string ContractConditions { get; set; }  

    }
}
