using ship_convenient.Constants.PackageConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Services.PackageService;
using unitofwork_core.Constant.Package;

namespace ship_convenient.BgService
{
    public class BgServiceSuccessPackage : BackgroundService
    {
        private readonly ILogger<BgServiceSuccessPackage> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public BgServiceSuccessPackage(ILogger<BgServiceSuccessPackage> logger, IServiceScopeFactory serviceProvider)
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
                    IUnitOfWork _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    IPackageService _packageService = scope.ServiceProvider.GetRequiredService<IPackageService>();
                    IConfigRepository _configRepository = _unitOfWork.Configs;
                    // int suggestTime = _configRepository.GetGapTimeSuggest();
                    int suggestTime = 60;
                    await ToSuccessPackageProcess(_unitOfWork, _packageService);
                    await Task.Delay(TimeSpan.FromSeconds(suggestTime), stoppingToken);
                }

            }
        }

        public async Task ToSuccessPackageProcess(
            IUnitOfWork unitOfWork, IPackageService packageService)
        {
            IPackageRepository packageRepo = unitOfWork.Packages;
            List<Package> deliveredPackages = await packageRepo.GetAllAsync(predicate: item =>
                item.Status == PackageStatus.DELIVERED_SUCCESS);
            deliveredPackages = deliveredPackages.Where(
                item => Utils.CompareEqualTimeHour(item.ModifiedAt.ToUniversalTime().AddDays(1), DateTime.UtcNow)).ToList();
            _logger.LogInformation($"{DateTime.UtcNow} - Số lượng gói hàng cần tự động chuyển trạng thái thành công: {deliveredPackages.Count}");
            foreach (Package package in deliveredPackages)
            {
                try
                {
                    ApiResponse response = await packageService.ToSuccessPackage(package.Id);
                    _logger.LogInformation(response.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception: {message}", ex.Message);
                }

            }

        }
    }
}
