namespace TimeCraft.Domain.Dtos.EmployeeDtos
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int? SalaryId { get; set; }

        public int PositionId { get; set; }
    }
}
