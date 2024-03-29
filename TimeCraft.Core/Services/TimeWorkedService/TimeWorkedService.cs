﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.TimeWorkedService
{
    public class TimeWorkedService : ITimeWorkedService<TimeWorked>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TimeWorkedService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TimeWorkedService(IMapper mapper, ILogger<TimeWorkedService> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<TimeWorked> GetById(int id)
        {
            var timeWorked = await _unitOfWork.Repository<TimeWorked>().GetById(x => x.Id == id).Where(x => !x.Deleted).FirstOrDefaultAsync();

            return timeWorked;
        }

        public async Task<IEnumerable<TimeWorked>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);

            var timeWorkeds = await _unitOfWork.Repository<TimeWorked>().GetByConditionPaginated(x => !x.Deleted, page, pageSize).ToListAsync();

            return timeWorkeds; throw new NotImplementedException();
        }

        public IQueryable<TimeWorked> SearchTimeWorkeds(int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            page = Math.Max(page, 1);

            var query = _unitOfWork.Repository<TimeWorked>().GetAll();

            query = query.Where(x => !x.Deleted);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }

        public async Task<int> Create(TimeWorked entityToCreate)
        {
            _unitOfWork.Repository<TimeWorked>().Create(entityToCreate);
            await _unitOfWork.CompleteAsync();

            return entityToCreate.Id;
        }

        public async Task Update(TimeWorked entityToUpdate)
        {
            var existingTimeWorked = await GetById(entityToUpdate.Id);
            if (existingTimeWorked is null)
            {
                throw new NullReferenceException("The timeWorked with the given id doesn't exist");
            }

            existingTimeWorked.WorkDate = entityToUpdate.WorkDate;
            existingTimeWorked.StartTime = entityToUpdate.StartTime;
            existingTimeWorked.EndTime = entityToUpdate.EndTime;
            existingTimeWorked.Duration = entityToUpdate.Duration;
            existingTimeWorked.ProjectId = entityToUpdate.ProjectId;
            existingTimeWorked.ProjectTaskId = entityToUpdate.ProjectTaskId;
            existingTimeWorked.Description = entityToUpdate.Description;

            existingTimeWorked.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.Repository<TimeWorked>().Update(existingTimeWorked);
            await _unitOfWork.CompleteAsync();
        }

        public async Task Delete(int id, bool softDelete = true)
        {
            var timeWorkedToBeDeleted = await GetById(id);
            if (timeWorkedToBeDeleted is null)
            {
                throw new NullReferenceException("The timeWorked with the given id doesn't exist");
            }

            if (softDelete)
            {
                timeWorkedToBeDeleted.Deleted = true;
                _unitOfWork.Repository<TimeWorked>().Update(timeWorkedToBeDeleted);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                _unitOfWork.Repository<TimeWorked>().Delete(timeWorkedToBeDeleted);
            }

            _logger.LogInformation($"{nameof(TimeWorkedService)} - Tried to {(softDelete ? "soft" : "hard")} delete timeWorked with id: {id}.");
            await _unitOfWork.CompleteAsync();
        }
    }
}