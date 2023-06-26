using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.PackageConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.PackageService;
using unitofwork_core.Constant.Package;

namespace ship_convenient.BgService
{
    public class BgServiceExpiredPackage : BackgroundService
    {
        private readonly ILogger<BgServiceExpiredPackage> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public BgServiceExpiredPackage(ILogger<BgServiceExpiredPackage> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    // Inject service
                    IUnitOfWork _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IPackageService _packageService = scope.ServiceProvider.GetRequiredService<IPackageService>();
                    IFirebaseCloudMsgService _fcmService = scope.ServiceProvider.GetRequiredService<IFirebaseCloudMsgService>();
                    await ExpriredPackagesProcess(_fcmService, _unitOfWork, _packageService);
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }

            }
        }

        public async Task ExpriredPackagesProcess(
            IFirebaseCloudMsgService fcmService, IUnitOfWork unitOfWork, IPackageService packageService)
        {
            IPackageRepository packageRepo = unitOfWork.Packages;
            INotificationRepository notificationRepo = unitOfWork.Notifications;
            ITransactionPackageRepository transactionPackage = unitOfWork.TransactionPackages;
            List<Package> packageStatusValid = await packageRepo.GetAllAsync(
                predicate: pk => pk.Status == PackageStatus.APPROVED || pk.Status == PackageStatus.WAITING,
                include: source => source.Include(pk => pk.Sender));
            packageStatusValid = packageStatusValid.Where(
                item => Utils.CompareEqualTimeHour(item.ExpiredTime.ToUniversalTime(), DateTime.UtcNow)).ToList();
            _logger.LogInformation("Số lượng gói hàng cần phải tự hủy: {count}", packageStatusValid.Count);
            foreach (Package package in packageStatusValid)
            {
                try
                {
                    Notification notificationSender = new Notification();
                    notificationSender.AccountId = package.SenderId;
                    notificationSender.Title = "Đơn hàng hết hạn";
                    notificationSender.Content = "Đơn hàng của bạn đã hết hạn, vui lòng đặt lại đơn hàng mới";
                    notificationSender.TypeOfNotification = TypeOfNotification.EXPIRED;
                    notificationSender.PackageId = package.Id;

                    TransactionPackage transaction = new TransactionPackage();
                    transaction.PackageId = package.Id;
                    transaction.FromStatus = package.Status;
                    transaction.ToStatus = PackageStatus.EXPIRED;
                    transaction.Description = "Đơn hàng bị hết hạn";

                    await transactionPackage.InsertAsync(transaction);
                    await notificationRepo.InsertAsync(notificationSender);
                    package.Status = PackageStatus.EXPIRED;
                    packageRepo.Update(package);
                    int result = await unitOfWork.CompleteAsync();
                    if (result > 0 && !string.IsNullOrEmpty(package?.Sender?.RegistrationToken)) {
                        ApiResponse response = await fcmService.SendNotification(notificationSender.ToSendFirebaseModel());
                        _logger.LogInformation("Đã gửi thông báo đến người giao hàng về thời gian nhận đơn hàng: {response}", response.Message);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Exception: {message}", ex.Message);
                }

            }

        }
    }
}
