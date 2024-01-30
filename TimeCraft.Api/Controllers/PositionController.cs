using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeCraft.Core.Services.PositionService;
using TimeCraft.Domain.Dtos.PositionDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class PositionController : Controller
    {
        private readonly IPositionService<Position> _positionService;
        private readonly ILogger<PositionController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// PositionController constructor
        /// </summary>
        /// <param name="positionService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public PositionController(IPositionService<Position> positionService, ILogger<PositionController> logger, IMapper mapper)
        {
            _positionService = positionService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific position by ID
        /// </summary>
        /// <param name="id">The ID of the position to retrieve</param>
        /// <returns>The specified position</returns>
        /// <response code="200">Returns the specified position</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified position is not found</response>
        /// <tags>Position</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("position")]
        public async Task<IActionResult> GetPosition(int id)
        {
            var position = await _positionService.GetById(id);
            if (position is null)
            {
                return NotFound();
            }

            return Ok(position);
        }

        /// <summary>
        /// Gets all positions in paginated form
        /// </summary>
        /// <returns>A list of positions</returns>
        /// <response code="200">Returns the list of all positions</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Position</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("positions")]
        public async Task<IActionResult> GetPositions(int page = 1, int pageSize = 10)
        {
            var positions = await _positionService.GetAllAsync(page, pageSize);

            return Ok(positions);
        }

        /// <summary>
        /// Creates a position!
        /// </summary>
        /// <param name="positionToCreate"></param>
        /// <returns></returns>
        [HttpPost("position")]
        public async Task<IActionResult> CreatePosition(PositionCreateDto positionToCreate)
        {
            var position = _mapper.Map<Position>(positionToCreate);
            var createdId = await _positionService.Create(position);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(PositionController)} - Couldn't create the position.");
                return BadRequest("Couldn't create the position, something went wrong");
            }

            _logger.LogInformation($"{nameof(PositionController)} - Created position successfully.");
            return Ok("Position is created successfully!");
        }

        /// <summary>
        /// Updates a position by id!
        /// </summary>
        /// <param name="positionToUpdate"></param>
        /// <returns></returns>
        /// 
        [HttpPut("position")]
        public async Task<IActionResult> UpdatePosition(PositionUpdateDto positionToUpdate)
        {
            try
            {
                var position = _mapper.Map<Position>(positionToUpdate);
                await _positionService.Update(_mapper.Map<Position>(position));

                _logger.LogInformation($"{nameof(PositionController)} - Updated position successfully, Id: {positionToUpdate.Id}.");
                return Ok("Position is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PositionController)} - Error when updating position, Id: {positionToUpdate.Id}.");
                return BadRequest("Couldn't update the position, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a position by id!
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("position")]
        public async Task<IActionResult> DeletePosition(int positionId, bool softDelete = true)
        {
            try
            {
                await _positionService.Delete(positionId, softDelete);
                
                _logger.LogInformation($"{nameof(PositionController)} - Deleted position successfully, Id: {positionId}.");
                return Ok("Position has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PositionController)} - Error when deleting position, Id: {positionId}.");
                return BadRequest("Couldn't delete the position, something went wrong");
            }
        }
    }
}
