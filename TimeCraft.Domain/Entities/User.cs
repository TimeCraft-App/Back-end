using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class User : BaseEntity
    {
        public new string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
