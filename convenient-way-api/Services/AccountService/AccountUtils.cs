using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using unitofwork_core.Constant.Package;
using Transaction = ship_convenient.Entities.Transaction;
using RouteEntity = ship_convenient.Entities.Route;
using ship_convenient.Helper.SuggestPackageHelper;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Services.AccountService
{
    public class AccountUtils : GenericService<AccountUtils>
    {
        private readonly ITransactionRepository _transactionRepo;
        public AccountUtils(ILogger<AccountUtils> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _transactionRepo = unitOfWork.Transactions;
        }

        public async Task<int> AvailableBalanceAsync(Guid accountId)
        {
            if (await IsNewAccountAsync(accountId))
            {
                int balanceDefault = _configRepo.GetDefaultBalanceNewAccount();
                return balanceDefault;
            };
            int result = 0;
            Account? account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null) throw new ArgumentException("Tài khoản không tồn tại");
            if (account.Role == RoleName.DELIVER)
            {
                List<string> validStatus = new() {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS, PackageStatus.DELIVERED_FAILED
                };
                List<Package> packages = await _packageRepo.GetAllAsync(
                    include: source => source.Include(p => p.Products),
                    predicate: p => validStatus.Contains(p.Status) && p.DeliverId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packages)
                {
                    totalBalanceNotAvaiable += item.Products.Sum(pro => pro.Price);
                }
                result = account.Balance - totalBalanceNotAvaiable;
            }
            else if (account.Role == RoleName.SENDER) {
                List<string> validStatusSender = new() {
                PackageStatus.APPROVED, PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS,
                };
                List<Package> packagesSender = await _packageRepo.GetAllAsync(
                    predicate: p => validStatusSender.Contains(p.Status) && p.SenderId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packagesSender)
                {
                    totalBalanceNotAvaiable += item.PriceShip;
                }
                result = account.Balance - totalBalanceNotAvaiable;
            }

            return result;
        }

        public async Task<ResponseBalanceModel> AvailableBalanceModel(Guid accountId)
        {
            ResponseBalanceModel result = new();
            if (await IsNewAccountAsync(accountId))
            {
                int balanceDefault = _configRepo.GetDefaultBalanceNewAccount();
                result.IsNewAccount = true;
                result.Balance = balanceDefault;
                return result;
            };
            Account? account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null) throw new ArgumentException("Tài khoản không tồn tại");
            if (account.Role == RoleName.DELIVER)
            {
                List<string> validStatus = new() {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS, PackageStatus.DELIVERED_FAILED
                };
                List<Package> packages = await _packageRepo.GetAllAsync(
                    include: source => source.Include(p => p.Products),
                    predicate: p => validStatus.Contains(p.Status) && p.DeliverId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packages)
                {
                    totalBalanceNotAvaiable += item.Products.Sum(pro => pro.Price);
                }
                result.Balance = account.Balance - totalBalanceNotAvaiable;
            }
            else if (account.Role == RoleName.SENDER)
            {
                List<string> validStatusSender = new() {
                PackageStatus.APPROVED, PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS,
                };
                List<Package> packagesSender = await _packageRepo.GetAllAsync(
                    predicate: p => validStatusSender.Contains(p.Status) && p.SenderId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packagesSender)
                {
                    totalBalanceNotAvaiable += item.PriceShip;
                }
                result.Balance = account.Balance - totalBalanceNotAvaiable;
            }
            return result;
        }

        public async Task<RouteEntity> GetActiveRoute(Guid accountId) {
            Account? account = await _accountRepo.GetByIdAsync(accountId, include: source => source.Include(ac => ac.InfoUser));
            if (account == null) throw new ArgumentException("Tài khoản không tồn tại");
            RouteEntity? route = await _routeRepo.FirstOrDefaultAsync(predicate: route => route.IsActive == true && route.InfoUserId == account.InfoUser.Id);
            if (route == null) throw new ArgumentException("Không tìm thấy tuyến đường");
            return route;
        }

        public async Task<bool> IsNewAccountAsync(Guid accountId)
        {
            bool result = false;
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == accountId || p.SenderId == accountId);
            List<Transaction> transactions = await _transactionRepo.GetAllAsync(
                predicate: t => t.AccountId == accountId);
            if (packages.Count == 0 && transactions.Count == 0) result = true;
            return result;
        }



        public bool IsNewAccount(Guid accountId)
        {
            bool result = false;
            List<Package> packages = _packageRepo.GetAll(
                predicate: p => p.DeliverId == accountId || p.SenderId == accountId);
            List<Transaction> transactions = _transactionRepo.GetAll(
                predicate: t => t.AccountId == accountId);
            if (packages.Count == 0 && transactions.Count == 0) result = true;
            return result;
        }

        public int AvailableBalance(Guid accountId)
        {
            if (IsNewAccount(accountId))
            {
                int balanceDefault = _configRepo.GetDefaultBalanceNewAccount();
                return balanceDefault;
            };
            int result = 0;
            Account? account = _accountRepo.GetById(accountId);
            if (account == null) throw new ArgumentException("Tài khoản không tồn tại");
            if (account.Role == RoleName.DELIVER)
            {
                List<string> validStatus = new() {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS, PackageStatus.DELIVERED_FAILED
                };
                List<Package> packages = _packageRepo.GetAll(
                    include: source => source.Include(p => p.Products),
                    predicate: p => validStatus.Contains(p.Status) && p.DeliverId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packages)
                {
                    totalBalanceNotAvaiable += item.Products.Sum(pro => pro.Price);
                }
                result = account.Balance - totalBalanceNotAvaiable;
            }
            else if (account.Role == RoleName.SENDER)
            {
                List<string> validStatusSender = new() {
                PackageStatus.APPROVED, PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS,
                };
                List<Package> packagesSender = _packageRepo.GetAll(
                    predicate: p => validStatusSender.Contains(p.Status) && p.SenderId == accountId);
                int totalBalanceNotAvaiable = 0;
                foreach (var item in packagesSender)
                {
                    totalBalanceNotAvaiable += item.PriceShip;
                }
                result = account.Balance - totalBalanceNotAvaiable;
            }

            return result;
        }

        public async Task<List<Account>> GetListAccountActive()
        {
            List<ConfigUser> configActives = await _configUserRepo.GetAllAsync(
                predicate: (config) => config.Name == DefaultUserConfigConstant.IS_ACTIVE && config.Value == "TRUE",
                include: source => source.Include(cf => cf.InfoUser).ThenInclude(info => info.Account));
            List<Account?> activeAccounts = configActives.Select(config => config.InfoUser.Account).ToList();
            return activeAccounts;
        }

        public async Task<Account> GetAdminBalance()
        {
            #region Predicate
            Expression<Func<Account, bool>> predicateAdminBalance = (acc) => acc.Role == RoleName.ADMIN_BALANCE;
            #endregion

            Account? adminBalance = await _accountRepo.FirstOrDefaultAsync(predicateAdminBalance, disableTracking: false);
            if (adminBalance == null) throw new ArgumentException("Không tìm thấy tài khoản quản lý tiền");
            return adminBalance;
        }

        public async Task<List<DistancePackageModel>> GetCurrentVirtualPoint(Guid deliverId) {
            List<DistancePackageModel> result;
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId, include: source => source.Include(ac => ac.InfoUser));
            Route activeRoute = await GetActiveRoute(deliverId);
            string suggestDirection = _configUserRepo.GetDirectionSuggest(deliver.InfoUser.Id); 
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == deliverId && (p.Status == PackageStatus.SELECTED || p.Status == PackageStatus.PICKUP_SUCCESS),
                include: source => source.Include(p => p.Products));
            result = SuggestPackageHelper.GetListPointOrderName(direction: suggestDirection, packages: packages, route: activeRoute);
            return result;
        }

    }
}
