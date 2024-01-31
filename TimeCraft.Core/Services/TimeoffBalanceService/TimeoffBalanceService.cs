using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;
using TimeCraft.Infrastructure.Constants;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.TimeoffBalanceService
{
    public class TimeoffBalanceService : ITimeoffBalanceService<TimeoffBalance>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TimeoffBalanceService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private const int TOTAL_TIMEOFF_DAYS = 40; // 20 vacation, 10 sick, 5 personal and 5 others

        public TimeoffBalanceService(IMapper mapper, ILogger<TimeoffBalanceService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<TimeoffBalance> GetById(int id)
        {
            var employee = await _unitOfWork.Repository<TimeoffBalance>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return employee;
        }

        public async Task<IEnumerable<TimeoffBalance>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var employees = await _unitOfWork.Repository<TimeoffBalance>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return employees; throw new NotImplementedException();
        }

        public IQueryable<TimeoffBalance> SearchTimeoffBalances(int employeeId, int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            page = Math.Max(page, 1);

            var query = _unitOfWork.Repository<TimeoffBalance>().GetAll();

            if (employeeId > 0)
            {
                query = query.Where(x => x.EmployeeId == employeeId);
            }

            query = query.Where(x => !x.Deleted);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }

        public async Task<int> Create(TimeoffBalance entityToCreate)
        {
            _unitOfWork.Repository<TimeoffBalance>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            return entityToCreate.Id;
        }

        public async Task Update(TimeoffBalance entityToUpdate)
        {
            var existingTimeoffBalance = await GetById(entityToUpdate.Id);
            if (existingTimeoffBalance is null)
            {
                throw new NullReferenceException("The employee with the given id doesn't exist");
            }

            existingTimeoffBalance.VacationDays = entityToUpdate.VacationDays;
            existingTimeoffBalance.SickDays = entityToUpdate.SickDays;
            existingTimeoffBalance.PersonalDays = entityToUpdate.PersonalDays;
            existingTimeoffBalance.OtherTimeOffDays = entityToUpdate.OtherTimeOffDays;

            existingTimeoffBalance.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<TimeoffBalance>().Update(existingTimeoffBalance);
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
                _unitOfWork.Repository<TimeoffBalance>().Update(employeeToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<TimeoffBalance>().Delete(employeeToBeDeleted);
            }

            _logger.LogInformation($"{nameof(TimeoffBalanceService)} - Tried to {(softDelete ? "soft" : "hard")} delete employee with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }


        /// <summary>
        /// Changes balance quantity for the given employee (can increase or decrease)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="quantity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task ChangeBalance(int employeeId, int quantity, TimeoffType type)
        {
            var balance = SearchTimeoffBalances(employeeId).FirstOrDefault();

            switch (type) {
                case TimeoffType.Vacation:
                    balance.VacationDays += quantity;
                    break;
                case TimeoffType.Sick: 
                    balance.SickDays += quantity;
                    break;
                case TimeoffType.Personal: 
                    balance.PersonalDays += quantity;   
                    break;
                case TimeoffType.Other:
                    balance.OtherTimeOffDays += quantity;
                    break;
                default:
                    throw new Exception("The given type balance doesn't exist!");
            }

            _unitOfWork.Repository<TimeoffBalance>().Update(balance);
            await _unitOfWork.CompleteAsync();
        }

        public int GetBalance(int employeeId, TimeoffType type)
        {
            var balance = SearchTimeoffBalances(employeeId).FirstOrDefault();

            switch (type)
            {
                case TimeoffType.Vacation:
                    return balance.VacationDays;
                case TimeoffType.Sick:
                    return balance.SickDays;
                case TimeoffType.Personal:
                    return balance.PersonalDays;
                case TimeoffType.Other:
                    return balance.OtherTimeOffDays;
                default:
                    throw new Exception("The given type balance doesn't exist!");
            }
        }

        public async Task<int> CalculateUsedDays(int employeeId) {
            var timeoffBalance = await _unitOfWork.Repository<TimeoffBalance>().GetByCondition(x => x.EmployeeId == employeeId).FirstOrDefaultAsync();

            if (timeoffBalance is null)
            {
                throw new NullReferenceException("There is no timeoffbalance for the given employee id!");
            }

            var totalLeftDays = timeoffBalance.VacationDays + timeoffBalance.SickDays + timeoffBalance.PersonalDays + timeoffBalance.OtherTimeOffDays;

            var usedDays = TOTAL_TIMEOFF_DAYS - totalLeftDays;

            return usedDays;
        }
    }
}
