using FirebaseAdmin.Messaging;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.NotificationModel;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using FcmNotification = FirebaseAdmin.Messaging.Notification;
using unitofwork_core.Constant.Package;
using ship_convenient.Constants.PackageConstant;
using Microsoft.EntityFrameworkCore;

namespace ship_convenient.Services.Notificationservice
{
    public class NotificationService :GenericService<NotificationService>, INotificationService
    {
        private readonly IFirebaseCloudMsgService _firebaseCloudMsgService;
        public NotificationService(ILogger<NotificationService> logger, IUnitOfWork unitOfWork, IFirebaseCloudMsgService firebaseCloudMsgService) : base(logger, unitOfWork)
        {
            this._firebaseCloudMsgService = firebaseCloudMsgService;
        }

        public async Task<ApiResponsePaginated<ResponseNotificationModel>> GetList(Guid accountId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseNotificationModel> response = new();
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (errorPaging != null) {
                response.ToFailedResponse(errorPaging);
            }
            bool isExistAccount = IsExistedAccount(accountId);
            if (!isExistAccount) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            PaginatedList<ResponseNotificationModel> result = await _notificationRepo.GetPagedListAsync(
                predicate: (source) => source.AccountId == accountId,
                selector: (source) => source.ToResponseModel(),
                orderBy: (source) => source.OrderByDescending(n => n.CreatedAt),
                pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(result, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse> NotificationTracking(NotificationTrackingModel model)
        {
            ApiResponse response = new();
            bool isExistAccount = IsExistedAccount(model.DeliverId);
            if (!isExistAccount) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
                return response;
            }

            #region Predicates
            List<Expression<Func<Package,bool>>> predicates = new();
            Expression<Func<Package, bool>> predicate = (source) => source.DeliverId == model.DeliverId;
            predicates.Add(predicate);
            List<string> validStatus = new List<string> {
                PackageStatus.SELECTED,
                PackageStatus.PICKUP_SUCCESS,
            };
            Expression<Func<Package, bool>> predicateStatus = (source) => validStatus.Contains(source.Status);
            predicates.Add(predicateStatus);
            #endregion
            List<Package> packages = await _packageRepo.GetAllAsync(predicates: predicates,
                include: (source) => source.Include(p => p.Sender));
            if (packages.Count == 0)
            {
                response.ToSuccessResponse("Không có gói hàng cần gửi thông báo");
                return response;
            }
            int count = packages.Count;
            #region send notification to senders
            int numberNotify = 0;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(packages[i].Sender?.RegistrationToken))
                    {
                        Message message = new Message();
                        message.Token = packages[i].Sender?.RegistrationToken;
                        message.Data = model.Data;
                        string responseFirebase = await _firebaseCloudMsgService.SendNotification(message);
                        if (!string.IsNullOrEmpty(responseFirebase))
                        {
                            numberNotify = numberNotify + 1;
                        }
                    }
                }
                catch (Exception e) {
                    // response.ToFailedResponse("Error when send notification to sender");
                    _logger.LogError(e, "Error when send notification to sender");
                }
               
            }
            response.ToSuccessResponse("Số lượng tin nhắn đã gửi: " + numberNotify);
            #endregion
            return response;
        }

       
    }
}
