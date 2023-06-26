using FirebaseAdmin.Messaging;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.FirebaseNotificationModel;

namespace ship_convenient.Services.FirebaseCloudMsgService
{
    public interface IFirebaseCloudMsgService
    {
        Task<ApiResponse> SendNotification(SendNotificationModel model);
        Task<string> SendNotification(Message model);
    }
}
