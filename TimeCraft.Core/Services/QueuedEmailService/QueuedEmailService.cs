using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.QueuedEmailService
{
    public class QueuedEmailService : IQueuedEmailService<QueuedEmail>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<QueuedEmailService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public QueuedEmailService(IMapper mapper, ILogger<QueuedEmailService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueuedEmail> GetById(int id)
        {
            var queuedEmail = await _unitOfWork.Repository<QueuedEmail>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return queuedEmail;
        }

        public async Task<IEnumerable<QueuedEmail>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var queuedEmails = await _unitOfWork.Repository<QueuedEmail>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return queuedEmails; throw new NotImplementedException();
        }

        public IQueryable<QueuedEmail> SearchQueuedEmails(int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            page = Math.Max(page, 1);

            var query = _unitOfWork.Repository<QueuedEmail>().GetAll();

            query = query.Where(x => !x.Deleted);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }

        public async Task<int> Create(QueuedEmail entityToCreate)
        {
            _unitOfWork.Repository<QueuedEmail>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            return entityToCreate.Id;
        }

        public async Task Update(QueuedEmail entityToUpdate)
        {
            var existingQueuedEmail = await GetById(entityToUpdate.Id);
            if (existingQueuedEmail is null)
            {
                throw new NullReferenceException("The queuedEmail with the given id doesn't exist");
            }

            existingQueuedEmail.CCs = entityToUpdate.CCs;
            existingQueuedEmail.Subject = entityToUpdate.Subject;
            existingQueuedEmail.Body = entityToUpdate.Body;

            existingQueuedEmail.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<QueuedEmail>().Update(existingQueuedEmail);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id, bool softDelete = true)
        {
            var queuedEmailToBeDeleted = await GetById(id);
            if (queuedEmailToBeDeleted is null)
            {
                throw new NullReferenceException("The queuedEmail with the given id doesn't exist");
            }

            if (softDelete)
            {
                queuedEmailToBeDeleted.Deleted = true;
                _unitOfWork.Repository<QueuedEmail>().Update(queuedEmailToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<QueuedEmail>().Delete(queuedEmailToBeDeleted);
            }

            _logger.LogInformation($"{nameof(QueuedEmailService)} - Tried to {(softDelete ? "soft" : "hard")} delete queuedEmail with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }
    }
}