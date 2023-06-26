using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.FeedbackModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.FeedbackService;

namespace ship_convenient.Controllers
{
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<AccountController> _logger;
        public FeedbackController(IFeedbackService feedbackService, ILogger<AccountController> logger)
        {
            _feedbackService = feedbackService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseFeedbackModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(Guid? packageId, Guid? creatorId,Guid? receiverId,string? feedbackFor, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponseFeedbackModel> response = await _feedbackService.GetList(
                    packageId, creatorId, receiverId, feedbackFor, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ResponseFeedbackModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateFeedbackModel model)
        {
            try
            {
                ApiResponse<ResponseFeedbackModel> response = await _feedbackService.Create(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ResponseFeedbackModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateFeedbackModel model)
        {
            try
            {
                ApiResponse<ResponseFeedbackModel> response = await _feedbackService.Update(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                ApiResponse response = await _feedbackService.Delete(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("rating/{accountId}")]
        [ProducesResponseType(typeof(ApiResponse<RatingAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRating(Guid accountId)
        {
            try
            {
                ApiResponse<RatingAccountModel> response = await _feedbackService.GetRating(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("rating-v2/{accountId}")]
        [ProducesResponseType(typeof(ApiResponse<RatingAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRatingV2(Guid accountId)
        {
            try
            {
                ApiResponse<RatingAccountModel> response = await _feedbackService.GetRatingV2(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }
    }
}
