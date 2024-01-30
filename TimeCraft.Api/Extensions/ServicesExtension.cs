using TimeCraft.Core.Services.EmployeeService;
using TimeCraft.Core.Services.LeaveManager;
using TimeCraft.Core.Services.PositionService;
using TimeCraft.Core.Services.SalaryService;
using TimeCraft.Core.Services.TimeoffBalanceService;
using TimeCraft.Core.Services.UserService;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Extensions
{
    public static class ServicesExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IEmployeeService<Employee>, EmployeeService>();
            services.AddTransient<IPositionService<Position>, PositionService>();
            services.AddTransient<ISalaryService<Salary>, SalaryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILeaveManagerService, LeaveManagerService>();
            services.AddTransient<ITimeoffBalanceService<TimeoffBalance>, TimeoffBalanceService>(); 
        }
    }
}
