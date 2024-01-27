using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TimeCraft.Core.Services.TimeWorkedService;
using TimeCraft.Domain.Dtos.TimeWorkedDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Controllers
{
    public class TimeWorkedController : BaseController
    {
        private readonly ITimeWorkedService<TimeWorked> _timeWorkedService;
        private readonly ILogger<TimeWorkedController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// TimeWorkedController constructor
        /// </summary>
        /// <param name="timeWorkedService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public TimeWorkedController(ITimeWorkedService<TimeWorked> timeWorkedService, ILogger<TimeWorkedController> logger, IMapper mapper)
        {
            _timeWorkedService = timeWorkedService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific timeWorked by ID
        /// </summary>
        /// <param name="id">The ID of the timeWorked to retrieve</param>
        /// <returns>The specified timeWorked</returns>
        /// <response code="200">Returns the specified timeWorked</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified timeWorked is not found</response>
        /// <tags>TimeWorked</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeWorked")]
        public async Task<IActionResult> GetTimeWorked(int id)
        {
            var timeWorked = await _timeWorkedService.GetById(id);
            if (timeWorked is null)
            {
                return NotFound();
            }

            return Ok(timeWorked);
        }

        /// <summary>
        /// Gets all timeWorkeds in paginated form
        /// </summary>
        /// <returns>A list of timeWorkeds</returns>
        /// <response code="200">Returns the list of all timeWorkeds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>TimeWorked</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeWorkeds")]
        public async Task<IActionResult> GetTimeWorkeds(int page = 1, int pageSize = 10)
        {
            var timeWorkeds = await _timeWorkedService.GetAllAsync(page, pageSize);

            return Ok(timeWorkeds);
        }

        /// <summary>
        /// Creates a timeWorked!
        /// </summary>
        /// <param name="timeWorkedToCreate"></param>
        /// <returns></returns>
        [HttpPost("timeWorked")]
        public async Task<IActionResult> CreateTimeWorked(TimeWorkedCreateDto timeWorkedToCreate)
        {
            var timeWorked = _mapper.Map<TimeWorked>(timeWorkedToCreate);
            var createdId = await _timeWorkedService.Create(timeWorked);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(TimeWorkedController)} - Couldn't create the timeWorked.");
                return BadRequest("Couldn't create the timeWorked, something went wrong");
            }

            _logger.LogInformation($"{nameof(TimeWorkedController)} - Created timeWorked successfully.");
            return Ok("TimeWorked is created successfully!");
        }

        /// <summary>
        /// Updates a timeWorked by id!
        /// </summary>
        /// <param name="timeWorkedToUpdate"></param>
        /// <returns></returns>
        [HttpPut("timeWorked")]
        public async Task<IActionResult> UpdateTimeWorked(TimeWorkedUpdateDto timeWorkedToUpdate)
        {
            try
            {
                var timeWorked = _mapper.Map<TimeWorked>(timeWorkedToUpdate);
                await _timeWorkedService.Update(_mapper.Map<TimeWorked>(timeWorked));

                _logger.LogInformation($"{nameof(TimeWorkedController)} - Updated timeWorked successfully, Id: {timeWorkedToUpdate.Id}.");
                return Ok("TimeWorked is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeWorkedController)} - Error when updating timeWorked, Id: {timeWorkedToUpdate.Id}.");
                return BadRequest("Couldn't update the timeWorked, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a timeWorked by id!
        /// </summary>
        /// <param name="timeWorkedId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("timeWorked")]
        public async Task<IActionResult> DeleteTimeWorked(int timeWorkedId, bool softDelete = true)
        {
            try
            {
                await _timeWorkedService.Delete(timeWorkedId, softDelete);

                _logger.LogInformation($"{nameof(TimeWorkedController)} - Deleted timeWorked successfully, Id: {timeWorkedId}.");
                return Ok("TimeWorked has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeWorkedController)} - Error when deleting timeWorked, Id: {timeWorkedId}.");
                return BadRequest("Couldn't delete the timeWorked, something went wrong");
            }
        }
    }
}
