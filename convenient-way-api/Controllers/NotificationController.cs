using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Model.NotificationModel;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.Notificationservice;

namespace ship_convenient.Controllers
{
    public class NotificationController : BaseApiController
    {
        private readonly IFirebaseCloudMsgService _firebaseCloudMsgService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;
        public NotificationController(IFirebaseCloudMsgService firebaseCloudMsgService, INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _firebaseCloudMsgService = firebaseCloudMsgService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost]
        [Route("send-notification")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationModel model)
        {
            ApiResponse respone =  await _firebaseCloudMsgService.SendNotification(model);
            return SendResponse(respone);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseNotificationModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponseNotificationModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(Guid accountId, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponseNotificationModel> response = await _notificationService.GetList(accountId, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Có lỗi xảy ra: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("send-tracking")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TrackingPackages([FromBody] NotificationTrackingModel model)
        {
            try
            {
                ApiResponse response = await _notificationService.NotificationTracking(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Có lỗi xảy ra: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
