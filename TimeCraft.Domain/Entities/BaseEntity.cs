using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }
       
        public bool Deleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime UpdatedOn { get; set;}
    }
}
