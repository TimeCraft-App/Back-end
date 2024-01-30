using TimeCraft.Domain.Enums;

namespace TimeCraft.Domain.Dtos.QueuedEmailDtos
{
    public class QueuedEmailCreateDto
    {
        public string RecipientEmail { get; set; }

        public string CCs { get; set; }

        public DateTime QueuedDate { get; set; } = DateTime.Now;

        public int SendTries { get; set; } = 1;

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Status { get; set; } = EmailStatus.Pending.ToString();
    }
}
