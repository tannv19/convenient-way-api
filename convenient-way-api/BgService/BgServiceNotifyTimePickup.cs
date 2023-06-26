using ship_convenient.Constants.PackageConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.PackageService;

namespace ship_convenient.BgService
{
    public class BgServiceNotifyTimePickup : BackgroundService
    {
        private readonly ILogger<BgServiceNotifyTimePickup> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public BgServiceNotifyTimePickup(ILogger<BgServiceNotifyTimePickup> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
  
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    // Inject service
                    IUnitOfWork _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IPackageService _packageService = scope.ServiceProvider.GetRequiredService<IPackageService>();
                    IFirebaseCloudMsgService _fcmService = scope.ServiceProvider.GetRequiredService<IFirebaseCloudMsgService>();
                    await PickUpServiceNotificationProcess(_fcmService, _unitOfWork, _packageService);
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }
            
            }
        }
        
        public async Task PickUpServiceNotificationProcess(
            IFirebaseCloudMsgService fcmService, IUnitOfWork unitOfWork, IPackageService packageService) {

            List<Package> packagesNearTimePickup = await packageService.GetPackagesNearTimePickup();
            packagesNearTimePickup = packagesNearTimePickup.Where(
                item => Utils.CompareEqualTime(item.PickupTimeOver, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)))).ToList();
            _logger.LogInformation("Số lượng gói hàng cần được thông báo pickup: {count}", packagesNearTimePickup.Count);
            foreach (Package package in packagesNearTimePickup)
            {
                if (string.IsNullOrEmpty(package?.Deliver?.RegistrationToken)) return;
                try
                {
                    SendNotificationModel model = new()
                    {
                        AccountId = package.DeliverId!.Value,
                        Title = "Thời gian nhận đơn",
                        Body = $"Bạn có đơn hàng đang chờ nhận: {package.Id}",
                        TypeOfNotification = TypeOfNotification.PICKUP_TIME,
                        Data = new Dictionary<string, string>()
                    {
                        {"packageId", package.Id.ToString()},
                        {"typeOfNotification", TypeOfNotification.PICKUP_TIME}
                    }
                    };
                    ApiResponse response = await fcmService.SendNotification(model);
                    _logger.LogInformation("Đã gửi thông báo đến người giao hàng về thời gian nhận đơn hàng: {response}", response.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Exception: {message}", ex.Message);
                }
               
            }

        }
    }
}
