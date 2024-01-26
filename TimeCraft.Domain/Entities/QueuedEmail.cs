using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Domain.Entities
{
    public class QueuedEmail : BaseEntity
    {
        public string RecipientEmail { get; set; }

        public string CCs { get; set; }

        public DateTime QueuedDate { get; set; }

        public int SendTries { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Status { get; set; }  // Email Status
    }
}
