using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.FirebaseCloudMsgService;
using FcmService = ship_convenient.Services.FirebaseCloudMsgService.FirebaseCloudMsgService;
using FirebaseMsgException = FirebaseAdmin.Messaging.FirebaseMessagingException;

namespace ship_convenient.Services.GenericService
{
    public class GenericService<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IConfigRepository _configRepo;
        protected readonly IAccountRepository _accountRepo;
        protected readonly IPackageRepository _packageRepo;
        protected readonly INotificationRepository _notificationRepo;
        protected readonly IConfigUserRepository _configUserRepo;
        protected readonly IRoutePointRepository _routePointRepo;
        protected readonly IRouteRepository _routeRepo;

        public GenericService(ILogger<T> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _accountRepo = unitOfWork.Accounts;
            _packageRepo = unitOfWork.Packages;
            _notificationRepo = unitOfWork.Notifications;
            _configRepo = unitOfWork.Configs;
            _configUserRepo = unitOfWork.ConfigUsers;
            _routePointRepo = unitOfWork.RoutePoints;
            _routeRepo = unitOfWork.Routes;
        }

        public string? VerifyPaging(int pageIndex, int pageSize) {
            if (pageIndex < 0)
            {
                return "Số trang phải lớn hơn hoặc bằng 0";
            }
            if (pageSize < 1)
            {
                return "Số phần tử của trang phải lớn hơn 0";
            }
            return null;
        }

        public bool IsExistedAccount(Guid id) {
            return _unitOfWork.Accounts.GetById(id: id) != null;
        }
        public bool IsExistedPackage(Guid id)
        {
            return _unitOfWork.Packages.GetById(id: id) != null;
        }

        public async Task<string?> SendNotificationToAccount(IFirebaseCloudMsgService _fcmService ,Notification notification)
        {
            try
            {
                string? registrationToken = (await _unitOfWork.Accounts.GetByIdAsync(id: notification.AccountId))?.RegistrationToken;
                if (string.IsNullOrEmpty(registrationToken))
                {
                    return "Người dùng không có token đăng kí trên firebase";
                }
                SendNotificationModel sentModel = notification.ToSendFirebaseModel();
                ApiResponse response = await _fcmService.SendNotification(model: sentModel);
                if (!response.Success) {
                    return "Không gửi được thông báo";
                }
                return null;
            }
            catch (FirebaseMsgException ex)
            {
                _logger.LogError($"Firebase exception: {ex.Message}");
                return "Token không đúng";
            }
            catch (Exception ex) {
                _logger.LogError($"Exception when notify: {ex.Message}");
                return "Lỗi không rõ";
            }
        }

        
    }
}
