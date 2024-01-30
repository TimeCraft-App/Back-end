namespace TimeCraft.Core.Dtos
{
    internal class TimeoffRequestHRDto
    {
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Comment { get; set; }
    }
}
