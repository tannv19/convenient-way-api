using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Entities;

namespace ship_convenient.Core.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        public IAccountRepository Accounts { get; private set; }

        public IConfigRepository Configs { get; private set; }

        public IDiscountRepository Discounts { get; private set; }

        public IPackageRepository Packages { get; private set; }

        public IInfoUserRepository InfoUsers { get; private set; }

        public INotificationRepository Notifications { get; private set; }

        public IProductRepository Products { get; private set; }
        public IDepositRepository Deposits { get; private set; }
        public ITransactionRepository Transactions { get; private set; }

        public IRouteRepository Routes { get; private set; }
        public IFeedbackRepository Feedbacks { get; private set; }

        public ITransactionPackageRepository TransactionPackages { get; private set; }

        public IVehicleRepository Vehicles { get; private set; }
        public IReportRepository Reports { get; private set; }

        public IConfigUserRepository ConfigUsers { get; private set; }
        public IRoutePointRepository RoutePoints { get; private set; }
        public IConfigPriceRepository ConfigPrices { get; private set; }
        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            Accounts = new AccountRepository(context, logger);
            Configs =  new ConfigRepository(context, logger);
            Discounts = new DiscountRepository(context, logger);
            Packages = new PackageRepository(context, logger);
            InfoUsers = new InfoUserRepository(context, logger);
            Notifications = new NotificationRepository(context, logger);
            Products = new ProductRepository(context, logger);
            Transactions = new TransactionRepository(context, logger);
            Routes = new RouteRepository(context, logger);
            TransactionPackages = new TransactionPackageRepository(context, logger);
            Vehicles = new VehicleRepository(context, logger);
            Deposits = new DepositRepository(context, logger);
            Feedbacks = new FeedbackRepository(context, logger);
            Reports = new ReportRepository(context, logger);
            ConfigUsers = new ConfigUserRepository(context, logger);
            RoutePoints = new RoutePointRepository(context, logger);
            ConfigPrices = new ConfigPriceRepository(context, logger);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

      
    }
}
