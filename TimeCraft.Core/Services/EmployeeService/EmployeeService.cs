using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService<Employee>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IMapper mapper, ILogger<EmployeeService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Employee> GetById(int id)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var employees = await _unitOfWork.Repository<Employee>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return employees; throw new NotImplementedException();
        }

        public IQueryable<Employee> SearchEmployees(int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            page = Math.Max(page, 1);

            var query = _unitOfWork.Repository<Employee>().GetAll();

            query = query.Where(x => !x.Deleted);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }

        public async Task<int> Create(Employee entityToCreate)
        {
            _unitOfWork.Repository<Employee>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            await CreateDefaultTimeBalance(entityToCreate.Id);

            return entityToCreate.Id;
        }

        public async Task Update(Employee entityToUpdate)
        {
            var existingEmployee = await GetById(entityToUpdate.Id);
            if (existingEmployee is null)
            {
                throw new NullReferenceException("The employee with the given id doesn't exist");
            }

            existingEmployee.UserId = entityToUpdate.UserId;
            existingEmployee.SalaryId = entityToUpdate.SalaryId;
            existingEmployee.PositionId = entityToUpdate.PositionId;

            existingEmployee.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<Employee>().Update(existingEmployee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id, bool softDelete = true)
        {
            var employeeToBeDeleted = await GetById(id);
            if (employeeToBeDeleted is null)
            {
                throw new NullReferenceException("The employee with the given id doesn't exist");
            }

            if (softDelete)
            {
                employeeToBeDeleted.Deleted = true;
                _unitOfWork.Repository<Employee>().Update(employeeToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<Employee>().Delete(employeeToBeDeleted);
            }

            _logger.LogInformation($"{nameof(EmployeeService)} - Tried to {(softDelete ? "soft" : "hard")} delete employee with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }


        // Adds the default time balance for new employees
        private async Task CreateDefaultTimeBalance(int employeeId)
        {
            var timeoffBalance = new TimeoffBalance
            {
                EmployeeId = employeeId, 
                VacationDays = 20, // 20 days on a year
                SickDays = 10, // 10 days on a year
                PersonalDays = 5, 
                OtherTimeOffDays = 5
            };

            _unitOfWork.Repository<TimeoffBalance>().Create(timeoffBalance);
            await _unitOfWork.CompleteAsync();
        }
    }
}
