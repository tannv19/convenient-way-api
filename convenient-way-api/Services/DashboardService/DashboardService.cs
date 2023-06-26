using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.DashboardModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using unitofwork_core.Constant.Package;

namespace ship_convenient.Services.DashboardService
{
    public class DashboardService : GenericService<DashboardService>, IDashboardService
    {
        private readonly AccountUtils _accountUtils;
        public DashboardService(ILogger<DashboardService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils) : base(logger, unitOfWork)
        {
            _accountUtils = accountUtils;
        }

        public async Task<ApiResponse<PackageCountModel>> GetCountPackage()
        {
            ApiResponse<PackageCountModel> response = new();
            List<Package> packages = await _packageRepo.GetAllAsync();
            int waitings = packages.Where(p => p.Status == PackageStatus.WAITING).Count();
            int approved = packages.Where(p => p.Status == PackageStatus.APPROVED).Count();
            int reject = packages.Where(p => p.Status == PackageStatus.REJECT).Count();
            int selected = packages.Where(p => p.Status == PackageStatus.SELECTED).Count();
            int pickupSuccess = packages.Where(p => p.Status == PackageStatus.PICKUP_SUCCESS).Count();
            int pickupFailed = packages.Where(p => p.Status == PackageStatus.PICKUP_FAILED).Count();
            int deliverCancel = packages.Where(p => p.Status == PackageStatus.DELIVER_CANCEL).Count();
            int senderCancel = packages.Where(p => p.Status == PackageStatus.SENDER_CANCEL).Count();
            int deliveredSuccess = packages.Where(p => p.Status == PackageStatus.DELIVERED_SUCCESS).Count();
            int deliveredFailed = packages.Where(p => p.Status == PackageStatus.DELIVERED_FAILED).Count();
            int refundSuccess = packages.Where(p => p.Status == PackageStatus.REFUND_TO_WAREHOUSE_SUCCESS).Count();
            int refundFailed = packages.Where(p => p.Status == PackageStatus.REFUND_TO_WAREHOUSE_FAILED).Count();
            int success = packages.Where(p => p.Status == PackageStatus.SUCCESS).Count();
            int expired = packages.Where(p => p.Status == PackageStatus.EXPIRED).Count();
            int all = packages.Count();
            PackageCountModel result = new PackageCountModel
            {
                All = all,
                Waiting = waitings,
                Approved = approved,
                Reject = reject,
                Selected = selected,
                PickupSuccess = pickupSuccess,
                PickupFailed = pickupFailed,
                DeliverCancel = deliverCancel,
                SenderCancel = senderCancel,
                DeliveredSuccess = deliveredSuccess,
                DeliveredFailed = deliveredFailed,
                RefundToWarehouseSuccess = refundSuccess,
                Expired = expired,
                RefundToWarehouseFailed = refundFailed,
                Success = success
            };
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;

        }

        public async Task<ApiResponse<PackageCountModel>> GetCountPackage(Guid? deliverId, Guid? senderId)
        {
            ApiResponse<PackageCountModel> response = new();
            List<Expression<Func<Package, bool>>> predicates = new();
            if (deliverId != null) predicates.Add(p => p.DeliverId == deliverId);
            if (senderId != null) predicates.Add(p => p.SenderId == senderId);
            List<Package> packages = await _packageRepo.GetAllAsync(predicates: predicates);

            int waitings = packages.Where(p => p.Status == PackageStatus.WAITING).Count();
            int approved = packages.Where(p => p.Status == PackageStatus.APPROVED).Count();
            int reject = packages.Where(p => p.Status == PackageStatus.REJECT).Count();
            int selected = packages.Where(p => p.Status == PackageStatus.SELECTED).Count();
            int pickupSuccess = packages.Where(p => p.Status == PackageStatus.PICKUP_SUCCESS).Count();
            int pickupFailed = packages.Where(p => p.Status == PackageStatus.PICKUP_FAILED).Count();
            int deliverCancel = packages.Where(p => p.Status == PackageStatus.DELIVER_CANCEL).Count();
            int senderCancel = packages.Where(p => p.Status == PackageStatus.SENDER_CANCEL).Count();
            int deliveredSuccess = packages.Where(p => p.Status == PackageStatus.DELIVERED_SUCCESS).Count();
            int deliveredFailed = packages.Where(p => p.Status == PackageStatus.DELIVERED_FAILED).Count();
            int refundSuccess = packages.Where(p => p.Status == PackageStatus.REFUND_TO_WAREHOUSE_SUCCESS).Count();
            int refundFailed = packages.Where(p => p.Status == PackageStatus.REFUND_TO_WAREHOUSE_FAILED).Count();
            int success = packages.Where(p => p.Status == PackageStatus.SUCCESS).Count();
            int expired = packages.Where(p => p.Status == PackageStatus.EXPIRED).Count();
            int all = packages.Count();
            PackageCountModel result = new PackageCountModel
            {
                All = all,
                Waiting = waitings,
                Approved = approved,
                Reject = reject,
                Selected = selected,
                PickupSuccess = pickupSuccess,
                PickupFailed = pickupFailed,
                DeliverCancel = deliverCancel,
                SenderCancel = senderCancel,
                DeliveredSuccess = deliveredSuccess,
                DeliveredFailed = deliveredFailed,
                RefundToWarehouseSuccess = refundSuccess,
                RefundToWarehouseFailed = refundFailed,
                Success = success,
                Expired = expired
            };
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<List<ResponseAccountModel>>> GetListAccountActive()
        {
            ApiResponse<List<ResponseAccountModel>> response = new();
            List<Account> activeAccounts = await _accountUtils.GetListAccountActive();
            List<ResponseAccountModel> result = activeAccounts.Select(acc => acc.ToResponseModel()).ToList();
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;
        }
    }
}
