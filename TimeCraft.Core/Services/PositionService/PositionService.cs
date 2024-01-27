using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.PositionService
{
    public class PositionService : IPositionService<Position>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PositionService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PositionService(IMapper mapper, ILogger<PositionService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Position> GetById(int id)
        {
            var position = await _unitOfWork.Repository<Position>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return position;
        }

        public async Task<IEnumerable<Position>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var positions = await _unitOfWork.Repository<Position>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return positions; throw new NotImplementedException();
        }

        public async Task<int> Create(Position entityToCreate)
        {
            _unitOfWork.Repository<Position>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            return entityToCreate.Id;
        }

        public async Task Update(Position entityToUpdate)
        {
            var existingPosition = await GetById(entityToUpdate.Id);
            if (existingPosition is null)
            {
                throw new NullReferenceException("The position with the given id doesn't exist");
            }

            existingPosition.Role = entityToUpdate.Role;
            existingPosition.Description = entityToUpdate.Description;
            existingPosition.ContractConditions = entityToUpdate.ContractConditions;

            existingPosition.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<Position>().Update(existingPosition);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id, bool softDelete = true)
        {
            var positionToBeDeleted = await GetById(id);
            if (positionToBeDeleted is null)
            {
                throw new NullReferenceException("The position with the given id doesn't exist");
            }

            if (softDelete)
            {
                positionToBeDeleted.Deleted = true;
                _unitOfWork.Repository<Position>().Update(positionToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<Position>().Delete(positionToBeDeleted);
            }

            _logger.LogInformation($"{nameof(PositionService)} - Tried to {(softDelete ? "soft" : "hard")} delete position with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }
    }
}
