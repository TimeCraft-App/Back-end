namespace TimeCraft.Domain.Dtos.TimeoffBalanceDtos
{
    public class TimeoffBalanceCreateDto
    {
        public int EmployeeId { get; set; }
        public int VacationDays { get; set; }
        public int SickDays { get; set; }
        public int PersonalDays { get; set; }
        public int OtherTimeOffDays { get; set; }
    }
}
