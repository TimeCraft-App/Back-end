using System;

namespace TimeCraft.EmailSender.Dtos
{
    public class TimeoffRequestStatusDto
    {
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string Type { get; set; } 

        public string FromStatus { get; set; }
        
        public string ToStatus { get; set; }    

        public string Comment { get; set; }

        public string Email { get; set; }
    }
}
