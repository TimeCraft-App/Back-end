using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TimeCraft.Core.Services.QueuedEmailService;
using TimeCraft.Domain.Dtos.QueuedEmailDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Controllers
{
    public class QueuedEmailController : BaseController
    {
        private readonly IQueuedEmailService<QueuedEmail> _queuedEmailService;
        private readonly ILogger<QueuedEmailController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// QueuedEmailController constructor
        /// </summary>
        /// <param name="queuedEmailService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public QueuedEmailController(IQueuedEmailService<QueuedEmail> queuedEmailService, ILogger<QueuedEmailController> logger, IMapper mapper)
        {
            _queuedEmailService = queuedEmailService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific queuedEmail by ID
        /// </summary>
        /// <param name="id">The ID of the queuedEmail to retrieve</param>
        /// <returns>The specified queuedEmail</returns>
        /// <response code="200">Returns the specified queuedEmail</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified queuedEmail is not found</response>
        /// <tags>QueuedEmail</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("queuedEmail")]
        public async Task<IActionResult> GetQueuedEmail(int id)
        {
            var queuedEmail = await _queuedEmailService.GetById(id);
            if (queuedEmail is null)
            {
                return NotFound();
            }

            return Ok(queuedEmail);
        }

        /// <summary>
        /// Gets all queuedEmails in paginated form
        /// </summary>
        /// <returns>A list of queuedEmails</returns>
        /// <response code="200">Returns the list of all queuedEmails</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>QueuedEmail</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("queuedEmails")]
        public async Task<IActionResult> GetQueuedEmails(int page = 1, int pageSize = 10)
        {
            var queuedEmails = await _queuedEmailService.GetAllAsync(page, pageSize);

            return Ok(queuedEmails);
        }


        /// <summary>
        /// Creates a queuedEmail!
        /// </summary>
        /// <param name="queuedEmailToCreate"></param>
        /// <returns></returns>
        [HttpPost("queuedEmail")]
        public async Task<IActionResult> CreateQueuedEmail(QueuedEmailCreateDto queuedEmailToCreate)
        {
            var queuedEmail = _mapper.Map<QueuedEmail>(queuedEmailToCreate);
            var createdId = await _queuedEmailService.Create(queuedEmail);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(QueuedEmailController)} - Couldn't create the queuedEmail.");
                return BadRequest("Couldn't create the queuedEmail, something went wrong");
            }

            _logger.LogInformation($"{nameof(QueuedEmailController)} - Created queuedEmail successfully.");
            return Ok("QueuedEmail is created successfully!");
        }

        /// <summary>
        /// Updates a queuedEmail by id!
        /// </summary>
        /// <param name="queuedEmailToUpdate"></param>
        /// <returns></returns>
        [HttpPut("queuedEmail")]
        public async Task<IActionResult> UpdateQueuedEmail(QueuedEmailUpdateDto queuedEmailToUpdate)
        {
            try
            {
                var queuedEmail = _mapper.Map<QueuedEmail>(queuedEmailToUpdate);
                await _queuedEmailService.Update(_mapper.Map<QueuedEmail>(queuedEmail));

                _logger.LogInformation($"{nameof(QueuedEmailController)} - Updated queuedEmail successfully, Id: {queuedEmailToUpdate.Id}.");
                return Ok("QueuedEmail is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(QueuedEmailController)} - Error when updating queuedEmail, Id: {queuedEmailToUpdate.Id}.");
                return BadRequest("Couldn't update the queuedEmail, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a queuedEmail by id!
        /// </summary>
        /// <param name="queuedEmailId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("queuedEmail")]
        public async Task<IActionResult> DeleteQueuedEmail(int queuedEmailId, bool softDelete = true)
        {
            try
            {
                await _queuedEmailService.Delete(queuedEmailId, softDelete);

                _logger.LogInformation($"{nameof(QueuedEmailController)} - Deleted queuedEmail successfully, Id: {queuedEmailId}.");
                return Ok("QueuedEmail has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(QueuedEmailController)} - Error when deleting queuedEmail, Id: {queuedEmailId}.");
                return BadRequest("Couldn't delete the queuedEmail, something went wrong");
            }
        }
    }
}
