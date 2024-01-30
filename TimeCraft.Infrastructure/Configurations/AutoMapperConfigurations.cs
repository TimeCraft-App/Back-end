using AutoMapper;
using TimeCraft.Domain.Dtos.EmployeeDtos;
using TimeCraft.Domain.Dtos.LeaveManagerDtos;
using TimeCraft.Domain.Dtos.PositionDtos;
using TimeCraft.Domain.Dtos.SalaryDtos;
using TimeCraft.Domain.Dtos.TimeoffBalanceDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Infrastructure.Configurations
{
    /// <summary>
    /// AutoMapperConfigurations
    /// </summary>
    public class AutoMapperConfigurations : Profile
    {
        /// <summary>
        /// AutoMapperConfigurations constructor
        /// </summary>
        public AutoMapperConfigurations()
        {
            CreateMap<EmployeeCreateDto, Employee>().ReverseMap();
            CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();

            CreateMap<PositionCreateDto, Position>().ReverseMap();
            CreateMap<PositionUpdateDto, Position>().ReverseMap();

            CreateMap<SalaryCreateDto, Salary>().ReverseMap();
            CreateMap<SalaryUpdateDto, Salary>().ReverseMap();

            CreateMap<SalaryCreateDto, Salary>().ReverseMap();
            CreateMap<SalaryUpdateDto, Salary>().ReverseMap();

            CreateMap<TimeoffBalanceCreateDto, TimeoffBalance>().ReverseMap();
            CreateMap<TimeoffBalanceUpdateDto, TimeoffBalance>().ReverseMap();

            CreateMap<TimeoffRequest, TimeoffRequestApplicationDto>().ReverseMap(); 
        }
    }
}
