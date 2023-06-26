using ship_convenient.Core.CoreModel;
using ship_convenient.Model.NotificationModel;

namespace ship_convenient.Services.Notificationservice
{
    public interface INotificationService
    {
        Task<ApiResponsePaginated<ResponseNotificationModel>> GetList(Guid accountId, int pageIndex, int pageSize);
        Task<ApiResponse> NotificationTracking(NotificationTrackingModel model);
    }
}
