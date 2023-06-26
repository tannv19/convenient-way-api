using FcmMessage = FirebaseAdmin.Messaging.Message;
using FcmNotification = FirebaseAdmin.Messaging.Notification;
using FcmFirebaseMsg = FirebaseAdmin.Messaging.FirebaseMessaging;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.FirebaseCloudMsgService
{
    public class FirebaseCloudMsgService : GenericService<FirebaseCloudMsgService>,IFirebaseCloudMsgService
    {
        
        public FirebaseCloudMsgService(ILogger<FirebaseCloudMsgService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
        }

        public async Task<ApiResponse> SendNotification(SendNotificationModel model) 
        {
            ApiResponse response = new();
            Account? account = await _accountRepo.GetByIdAsync(model.AccountId);
            if (account == null) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            FcmMessage message = new FcmMessage();
            message.Token = account?.RegistrationToken;
            if (model.Data != null) { 
                 message.Data = model.Data;
            }
            message.Notification = new FcmNotification()
            {
                Title = model.Title,
                Body = model.Body,
            };
            string responseFirebase = await FcmFirebaseMsg.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Response firebase notification: {response}");
            if (!string.IsNullOrEmpty(responseFirebase))
            {
                response.ToSuccessResponse($"Gửi thông báo thành công - {responseFirebase}");
            }
            else {
                response.ToFailedResponse($"Gửi thông báo thất bại");
            }
            return response;
        }

        public async Task<string> SendNotification(FcmMessage message)
        {
            string response = await FcmFirebaseMsg.DefaultInstance.SendAsync(message);
            return response;
        }
    }
}
