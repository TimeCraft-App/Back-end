using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TimeCraft.Core.Services.TimeWorkedService;
using TimeCraft.Domain.Dtos.TimeWorkedDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Controllers
{
    public class TimeWorkedController : BaseController
    {
        private readonly ITimeWorkedService<TimeWorked> _timeworkedService;
        private readonly ILogger<TimeWorkedController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// TimeWorkedController constructor
        /// </summary>
        /// <param name="timeworkedService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public TimeWorkedController(ITimeWorkedService<TimeWorked> timeworkedService, ILogger<TimeWorkedController> logger, IMapper mapper)
        {
            _timeworkedService = timeworkedService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific timeworked by ID
        /// </summary>
        /// <param name="id">The ID of the timeworked to retrieve</param>
        /// <returns>The specified timeworked</returns>
        /// <response code="200">Returns the specified timeworked</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified timeworked is not found</response>
        /// <tags>TimeWorked</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeworked")]
        public async Task<IActionResult> GetTimeWorked(int id)
        {
            var timeworked = await _timeworkedService.GetById(id);
            if (timeworked is null)
            {
                return NotFound();
            }

            return Ok(timeworked);
        }

        /// <summary>
        /// Gets all timeworkeds in paginated form
        /// </summary>
        /// <returns>A list of timeworkeds</returns>
        /// <response code="200">Returns the list of all timeworkeds</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>TimeWorked</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeworkeds")]
        public async Task<IActionResult> GetTimeWorkeds(int page = 1, int pageSize = 10)
        {
            var timeworkeds = await _timeworkedService.GetAllAsync(page, pageSize);

            return Ok(timeworkeds);
        }


        /// <summary>
        /// Creates a timeworked!
        /// </summary>
        /// <param name="timeworkedToCreate"></param>
        /// <returns></returns>
        [HttpPost("timeworked")]
        public async Task<IActionResult> CreateTimeWorked(TimeWorkedCreateDto timeworkedToCreate)
        {
            var timeworked = _mapper.Map<TimeWorked>(timeworkedToCreate);
            var createdId = await _timeworkedService.Create(timeworked);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(TimeWorkedController)} - Couldn't create the timeworked.");
                return BadRequest("Couldn't create the timeworked, something went wrong");
            }

            _logger.LogInformation($"{nameof(TimeWorkedController)} - Created timeworked successfully.");
            return Ok("TimeWorked is created successfully!");
        }

        /// <summary>
        /// Updates a timeworked by id!
        /// </summary>
        /// <param name="timeworkedToUpdate"></param>
        /// <returns></returns>
        [HttpPut("timeworked")]
        public async Task<IActionResult> UpdateTimeWorked(TimeWorkedUpdateDto timeworkedToUpdate)
        {
            try
            {
                var timeworked = _mapper.Map<TimeWorked>(timeworkedToUpdate);
                await _timeworkedService.Update(_mapper.Map<TimeWorked>(timeworked));

                _logger.LogInformation($"{nameof(TimeWorkedController)} - Updated timeworked successfully, Id: {timeworkedToUpdate.Id}.");
                return Ok("TimeWorked is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeWorkedController)} - Error when updating timeworked, Id: {timeworkedToUpdate.Id}.");
                return BadRequest("Couldn't update the timeworked, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a timeworked by id!
        /// </summary>
        /// <param name="timeworkedId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("timeworked")]
        public async Task<IActionResult> DeleteTimeWorked(int timeworkedId, bool softDelete = true)
        {
            try
            {
                await _timeworkedService.Delete(timeworkedId, softDelete);

                _logger.LogInformation($"{nameof(TimeWorkedController)} - Deleted timeworked successfully, Id: {timeworkedId}.");
                return Ok("TimeWorked has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeWorkedController)} - Error when deleting timeworked, Id: {timeworkedId}.");
                return BadRequest("Couldn't delete the timeworked, something went wrong");
            }
        }
    }

}
