namespace TimeCraft.EmailSender.EmailSender
{
    public class SendGridConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string SourceEmail { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public bool EnableClickTracking { get; set; } = false;
    }
}
