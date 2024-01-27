namespace TimeCraft.Domain.Dtos.SalaryDtos
{
    public class SalaryCreateDto
    {
        public double GrossSalary { get; set; }

        public double NetoSalary { get; set; }

        public string ContractType { get; set; }

        public int PositionId { get; set; }
    }
}
