using TimeCraft.Core.Services.EmployeeService;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Extensions
{
    public static class ServicesExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IEmployeeService<Employee>, EmployeeService>();
        }
    }
}
