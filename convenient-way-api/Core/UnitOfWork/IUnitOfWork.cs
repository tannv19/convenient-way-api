using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IConfigRepository Configs { get; }
        IDiscountRepository Discounts { get; }
        IPackageRepository Packages { get; }
        IInfoUserRepository InfoUsers { get; }
        INotificationRepository Notifications { get; }
        IProductRepository Products { get; }
        ITransactionRepository Transactions { get; }
        IRouteRepository Routes { get; }
        ITransactionPackageRepository TransactionPackages { get; }
        IVehicleRepository Vehicles { get; }
        IDepositRepository Deposits { get; }
        IFeedbackRepository Feedbacks { get; }
        IReportRepository Reports { get; }
        IConfigUserRepository ConfigUsers { get; }
        IRoutePointRepository RoutePoints { get; }
        IConfigPriceRepository ConfigPrices { get; }
        Task<int> CompleteAsync();
        int Complete();
    }
}
