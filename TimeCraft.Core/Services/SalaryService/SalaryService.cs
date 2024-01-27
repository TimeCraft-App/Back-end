using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.SalaryService
{
    public class SalaryService : ISalaryService<Salary>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SalaryService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SalaryService(IMapper mapper, ILogger<SalaryService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Salary> GetById(int id)
        {
            var salary = await _unitOfWork.Repository<Salary>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return salary;
        }

        public async Task<IEnumerable<Salary>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var salarys = await _unitOfWork.Repository<Salary>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return salarys; throw new NotImplementedException();
        }

        public async Task<int> Create(Salary entityToCreate)
        {
            _unitOfWork.Repository<Salary>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            return entityToCreate.Id;
        }

        public async Task Update(Salary entityToUpdate)
        {
            var existingSalary = await GetById(entityToUpdate.Id);
            if (existingSalary is null)
            {
                throw new NullReferenceException("The salary with the given id doesn't exist");
            }

            existingSalary.GrossSalary = entityToUpdate.GrossSalary;
            existingSalary.NetoSalary = entityToUpdate.NetoSalary;

            existingSalary.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<Salary>().Update(existingSalary);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id, bool softDelete = true)
        {
            var salaryToBeDeleted = await GetById(id);
            if (salaryToBeDeleted is null)
            {
                throw new NullReferenceException("The salary with the given id doesn't exist");
            }

            if (softDelete)
            {
                salaryToBeDeleted.Deleted = true;
                _unitOfWork.Repository<Salary>().Update(salaryToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<Salary>().Delete(salaryToBeDeleted);
            }

            _logger.LogInformation($"{nameof(SalaryService)} - Tried to {(softDelete ? "soft" : "hard")} delete salary with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }
    }
}
