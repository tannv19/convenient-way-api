using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Constants.DatimeConstant;
using ship_convenient.Constants.PackageConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.ReportModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.GenericService;
using ship_convenient.Services.MapboxService;
using System.Linq.Expressions;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using unitofwork_core.Constant.Transaction;
using unitofwork_core.Model.PackageModel;
using unitofwork_core.Model.ProductModel;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Services.PackageService
{
    public class PackageServiceScript : GenericService<PackageService>, IPackageServiceScript
    {
        private readonly ITransactionPackageRepository _transactionPackageRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IRouteRepository _routeRepo;
        private readonly IMapboxService _mapboxService;
        private readonly IFirebaseCloudMsgService _fcmService;
        private readonly PackageUtils _packageUtils;
        private readonly AccountUtils _accountUtils;

        public PackageServiceScript(ILogger<PackageService> logger, IUnitOfWork unitOfWork,
            IMapboxService mapboxService, IFirebaseCloudMsgService fcmService,
            PackageUtils packageUtils, AccountUtils accountUtils) : base(logger, unitOfWork)
        {
            _transactionPackageRepo = unitOfWork.TransactionPackages;
            _transactionRepo = unitOfWork.Transactions;
            _routeRepo = unitOfWork.Routes;

            _mapboxService = mapboxService;
            _fcmService = fcmService;
            _packageUtils = packageUtils;
            _accountUtils = accountUtils;
        }

        public async Task<ApiResponse> ApprovedPackage(Package package, bool isNotify)
        {
            ApiResponse response = new ApiResponse();

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.APPROVED;
            history.Description = "Đơn hàng đã được duyệt";
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = PackageStatus.APPROVED;
            #region Create notification for sender
            Notification notificationSender = new Notification();
            notificationSender.Title = "Đơn hàng đã được duyệt";
            notificationSender.Content = "Đơn hàng của bạn đã được duyệt";
            notificationSender.AccountId = package.SenderId;
            notificationSender.TypeOfNotification = TypeOfNotification.APPROVED;
            await _notificationRepo.InsertAsync(notificationSender);
            #endregion
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0 && isNotify)
            {
                await _packageUtils.NotificationValidUserWithPackageV2(package);
            }
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? $"Duyệt đơn thành công" : "Duyệt đơn thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model)
        {
            ApiResponse<ResponsePackageModel> response = new();

            Package package = model.ConverToEntity();
            await _packageRepo.InsertAsync(package);

            #region Create history
            TransactionPackage history = new();
            history.FromStatus = PackageStatus.NOT_EXIST;
            history.ToStatus = PackageStatus.WAITING;
            history.Description = "Đơn hàng được tạo";
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Tạo đơn hàng thành công" : "Tạo đơn thất bại";
            response.Data = result > 0 ? package.ToResponseModel() : null;
            #endregion

            return response;
        }

        public Task<ApiResponse> DeliverCancelPackage(Guid packageId, string? reason)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> DeliveredFailed(DeliveredFailedModel model)
        {
            ApiResponse response = new ApiResponse();
            Package? package = await _packageRepo.GetByIdAsync(model.PackageId, disableTracking: false,
                include: source => source.Include(p => p.Deliver).Include(p => p.Sender));

            #region Predicate
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = package?.Deliver;
            Account? sender = package?.Sender;
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicateAdminBalance, disableTracking: false);

            #region Create transactions
            Transaction systemTrans = new Transaction();
            systemTrans.Title = TransactionTitle.DELIVERED_FAILED;
            systemTrans.Description = $"Kiện hàng bị giao thất bại";
            systemTrans.Status = TransactionStatus.ACCOMPLISHED;
            systemTrans.TransactionType = TransactionType.INCREASE;
            systemTrans.CoinExchange = package.PriceShip;
            systemTrans.BalanceWallet = adminBalance.Balance + package.PriceShip;
            systemTrans.PackageId = package.Id;
            systemTrans.AccountId = adminBalance.Id;
            _logger.LogInformation($"System transaction: {systemTrans.CoinExchange}, Balance: {systemTrans.BalanceWallet}");

            Transaction senderTrans = new Transaction();
            senderTrans.Title = TransactionTitle.DELIVERED_FAILED;
            senderTrans.Description = "Kiện hàng của đã giao thất bại";
            senderTrans.Status = TransactionStatus.ACCOMPLISHED;
            senderTrans.TransactionType = TransactionType.DECREASE;
            senderTrans.CoinExchange = -package.PriceShip;
            senderTrans.BalanceWallet = sender.Balance - package.PriceShip;
            senderTrans.PackageId = package.Id;
            senderTrans.AccountId = sender.Id;
            _logger.LogInformation($"Shop transaction: {senderTrans.CoinExchange}, Balance: {senderTrans.BalanceWallet}");

            adminBalance.Balance = systemTrans.BalanceWallet;
            sender.Balance = senderTrans.BalanceWallet;

            List<Transaction> transactions = new List<Transaction> {
                    systemTrans, senderTrans
                };
            await _transactionRepo.InsertAsync(transactions);
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.DELIVERED_FAILED;
            history.Description = "Kiện hàng giao không thành công";
            history.Reason = model.Reason;
            history.ImageUrl = model.ImageUrl;
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.DELIVERED_FAILED;
            #endregion

            int result = await _unitOfWork.CompleteAsync();

            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Yêu cầu thành công" : "Yêu cầu thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliveredSuccess(Package package)
        {
            ApiResponse response = new ApiResponse();
            decimal profitPercent = decimal.Parse(_configRepo.GetValueConfig(ConfigConstant.PROFIT_PERCENTAGE)) / 100;

            #region Predicate
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = package?.Deliver;
            Account? sender = package?.Sender;
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicateAdminBalance, disableTracking: false);

            int totalPrice = 0;
            package.Products.ToList().ForEach(pr =>
            {
                totalPrice += pr.Price;
            });

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.DELIVERED_SUCCESS;
            history.Description = $"Kiện hàng đã được giao";
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            package.Status = PackageStatus.DELIVERED_SUCCESS;
            #endregion
           
            int result = await _unitOfWork.CompleteAsync();
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Yêu cầu thành công" : "Yêu cầu thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliverSelectedPackages(Guid deliverId, List<Package> packages)
        {
            ApiResponse response = new ApiResponse();

            #region Includable account, pakage
            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> includeDeliver = (source) => source.Include(sh => sh.InfoUser);
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> includePackage = (source) => source.Include(sh => sh.Products);
            #endregion
            #region Predicate system admin
            Expression<Func<Account, bool>> predicateSystemAdmin = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? deliver = await _accountRepo.GetByIdAsync(deliverId, include: includeDeliver, disableTracking: false);

            #region Verify params
            decimal totalPrice = 0;
            for (int i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                package.Products.ToList().ForEach(pr =>
                {
                    totalPrice += pr.Price;
                });
            }
            int availableBalance = await _accountUtils.AvailableBalanceAsync(deliverId);
            if (deliver == null || availableBalance < totalPrice)
            {
                response.ToFailedResponse($"Số dư khả dụng {availableBalance} không đủ để thực hiện nhận gói hàng");
                return response;
            }
            #endregion
            
            #region Create history
            for (int i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                package.DeliverId = deliverId;
                TransactionPackage history = new TransactionPackage();
                history.FromStatus = package.Status;
                history.ToStatus = PackageStatus.SELECTED;
                history.Description = "Kiện hàng đã được nhận, đang trên đường lấy hàng";
                history.PackageId = package.Id;
                await _transactionPackageRepo.InsertAsync(history);
                package.Status = history.ToStatus;
            }
            #endregion

            int result = await _unitOfWork.CompleteAsync();
            #region Send notification to senders
            #endregion
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Chọn đơn thành công" : "Chọn đơn thất bại";
            #endregion

            return response;
        }

        public async Task<ApiResponse> DeliverSelectedPackage(Guid deliverId, Package package)
        {
            ApiResponse response = new ApiResponse();

       
            #region Predicate system admin
            Expression<Func<Account, bool>> predicateSystemAdmin = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            #region Verify params
            decimal totalPrice = 0;

            #endregion

            #region Create history
            package.DeliverId = deliverId;
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.SELECTED;
            history.Description = "Kiện hàng đã được nhận, đang trên đường lấy hàng";
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            package.Status = history.ToStatus;
            #endregion

            int result = await _unitOfWork.CompleteAsync();
            #region Send notification to senders
            #endregion
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Chọn đơn thành công" : "Chọn đơn thất bại";
            #endregion

            return response;
        }


        public Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid senderId, string? status)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ResponsePackageModel>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? senderId, string? id, string? status, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> PickupPackageFailed(PickupPackageFailedModel model)
        {
            ApiResponse response = new ApiResponse();

            #region Includable pakage
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> includePackage =
                (source) => source.Include(pk => pk.Products).Include(pk => pk.Deliver).Include(pk => pk.Sender);
            #endregion

            Package? package = await _packageRepo.GetByIdAsync(model.PackageId, include: includePackage, disableTracking: false);
            Account? sender = package?.Sender;

            #region Create transactions
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicateAdminBalance, disableTracking: false);

            Transaction systemTrans = new Transaction();
            systemTrans.Title = TransactionTitle.PICKUP_FAILED;
            systemTrans.Description = $"Thất bại khi lấy kiện hàng";
            systemTrans.Status = TransactionStatus.ACCOMPLISHED;
            systemTrans.TransactionType = TransactionType.INCREASE;
            systemTrans.CoinExchange = package.PriceShip;
            systemTrans.BalanceWallet = adminBalance.Balance + package.PriceShip;
            systemTrans.PackageId = package.Id;
            systemTrans.AccountId = adminBalance.Id;
            _logger.LogInformation($"System transaction: {systemTrans.CoinExchange}, Balance: {systemTrans.BalanceWallet}");

            Transaction senderTrans = new Transaction();
            senderTrans.Title = TransactionTitle.PICKUP_FAILED;
            senderTrans.Description = "Lấy kiện hàng thất bại";
            senderTrans.Status = TransactionStatus.ACCOMPLISHED;
            senderTrans.TransactionType = TransactionType.DECREASE;
            senderTrans.CoinExchange = -package.PriceShip;
            senderTrans.BalanceWallet = sender.Balance - package.PriceShip;
            senderTrans.PackageId = package.Id;
            senderTrans.AccountId = sender.Id;
            _logger.LogInformation($"Sender transaction: {senderTrans.CoinExchange}, Balance: {senderTrans.BalanceWallet}");

            adminBalance.Balance = systemTrans.BalanceWallet;
            sender.Balance = senderTrans.BalanceWallet;
            List<Transaction> transactions = new List<Transaction> {
                    systemTrans, senderTrans
                };
            await _transactionRepo.InsertAsync(transactions);
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.PICKUP_FAILED;
            history.Description = $"Người giao không thể lấy được kiện hàng";
            history.Reason = model.Reason;
            history.ImageUrl = model.ImageUrl;
            history.PackageId = package.Id;
            package.Status = PackageStatus.PICKUP_FAILED;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse("Lấy hàng thành công");
            }
            else
            {
                response.ToFailedResponse("Lỗi không xác định");
            }

            return response;
        }

        public async Task<ApiResponse> PickupPackageSuccess(Package package)
        {
            ApiResponse response = new ApiResponse();

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.PICKUP_SUCCESS;
            history.Description = $"Kiện hàng được lấy thành công và đang trên đường giao hàng";
            history.PackageId = package.Id;
            package.Status = PackageStatus.PICKUP_SUCCESS;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            
            int result = await _unitOfWork.CompleteAsync();
         
            response.ToSuccessResponse("Lấy hàng thành công");
            return response;
        }

        public Task<ApiResponse> RefundToWarehouseFailed(Guid packageId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> RefundToWarehouseSuccess(Package package)
        {
            ApiResponse resposne = new ApiResponse();
            Account? sender = package?.Sender;
            Account? deliver = package?.Deliver;
            Account adminBalance = await _accountUtils.GetAdminBalance();
          

            #region Create transactions
            Transaction systemTrans = new Transaction();
            systemTrans.Title = TransactionTitle.RETURN_TO_WAREHOUSE_SUCCESS;
            systemTrans.Description = $"Kiện hàng đã được hoàn trả về kho";
            systemTrans.Status = TransactionStatus.ACCOMPLISHED;
            systemTrans.TransactionType = TransactionType.DECREASE;
            systemTrans.CoinExchange = -package.PriceShip;
            systemTrans.BalanceWallet = adminBalance.Balance - package.PriceShip;
            systemTrans.PackageId = package.Id;
            systemTrans.AccountId = adminBalance.Id;
            _logger.LogInformation($"System transaction: {systemTrans.CoinExchange}, Balance: {systemTrans.BalanceWallet}");

            Transaction deliverTrans = new Transaction();
            deliverTrans.Title = TransactionTitle.DELIVERED_FAILED;
            deliverTrans.Description = "Đơn hàng được hoàn trả về kho thành công";
            deliverTrans.Status = TransactionStatus.ACCOMPLISHED;
            deliverTrans.TransactionType = TransactionType.INCREASE;
            deliverTrans.CoinExchange = package.PriceShip;
            deliverTrans.BalanceWallet = deliver.Balance + package.PriceShip;
            deliverTrans.PackageId = package.Id;
            deliverTrans.AccountId = deliver.Id;
            _logger.LogInformation($"Deliver transaction: {deliverTrans.CoinExchange}, Balance: {deliverTrans.BalanceWallet}");

            adminBalance.Balance = systemTrans.BalanceWallet;
            sender.Balance = deliverTrans.BalanceWallet;

            List<Transaction> transactions = new List<Transaction> {
                    systemTrans, deliverTrans
                };
            await _transactionRepo.InsertAsync(transactions);
            #endregion

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.REFUND_TO_WAREHOUSE_SUCCESS;
            history.Description = "Kiện hàng đã được hoàn trả về kho trung tâm";
            history.PackageId = package.Id;
            package.Status = PackageStatus.REFUND_TO_WAREHOUSE_SUCCESS;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion

           
            int result = await _unitOfWork.CompleteAsync();
           
            return resposne;
        }

        public async Task<ApiResponse> RejectPackage(Package package)
        {
            ApiResponse response = new ApiResponse();

            #region Create history
            TransactionPackage history = new TransactionPackage();
            history.FromStatus = package.Status;
            history.ToStatus = PackageStatus.REJECT;
            history.Description = "Đơn hàng bị từ chối vào lúc: " + DateTime.UtcNow.ToString(DateTimeFormat.DEFAULT);
            history.PackageId = package.Id;
            await _transactionPackageRepo.InsertAsync(history);
            #endregion
            package.Status = PackageStatus.REJECT;
            #region Create notification to sender
            Notification notification = new Notification();
            notification.Title = "Đơn hàng bị từ chối";
            notification.Content = "Đơn hàng của bạn đã bị từ chối";
            notification.AccountId = package.SenderId;
            notification.TypeOfNotification = TypeOfNotification.REJECT;
            await _notificationRepo.InsertAsync(notification);
            #endregion
            int result = await _unitOfWork.CompleteAsync();
            #region Response result
            response.Success = result > 0 ? true : false;
            response.Message = result > 0 ? "Từ chối gói hàng thành công" : "Từ chối gói hàng thất bại thất bại";
            #endregion

            return response;
        }

        public Task<ApiResponse> ReportProblem(CreateReportPackageModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> SenderCancelPackage(Guid packageId, string? reason)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ToSuccessPackage(Guid packageId)
        {
            throw new NotImplementedException();
        }
    }
}