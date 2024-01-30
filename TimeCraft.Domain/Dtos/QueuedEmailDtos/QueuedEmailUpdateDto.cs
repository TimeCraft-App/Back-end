namespace TimeCraft.Domain.Dtos.QueuedEmailDtos
{
    public class QueuedEmailUpdateDto
    {
        public int Id { get; set; }

        public string CCs { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
