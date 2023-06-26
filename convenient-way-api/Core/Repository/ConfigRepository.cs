using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;
using unitofwork_core.Constant.ConfigConstant;

namespace ship_convenient.Core.Repository
{
    public class ConfigRepository : GenericRepository<ConfigApp>, IConfigRepository
    {
        public ConfigRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public int GetDefaultBalanceNewAccount()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.DEFAULT_BALANCE_NEW_ACCOUNT));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetMaxPickupSameTime()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.MAX_PICKUP_SAME_TIME));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetMaxRouteCreate()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.MAX_ROUTE_CREATE));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetMaxCancelInDay()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.MAX_CANCEL_IN_DAY));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetMaxSuggestCombo()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.MAX_SUGGEST_COMBO));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetMinimumDistance()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.MINIMUM_DISTANCE));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetProfitPercentage()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.PROFIT_PERCENTAGE));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetProfitPercentageRefund()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.PROFIT_PERCENTAGE_REFUND));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public string GetValueConfig(string configName)
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(configName));
            if (configApp != null)
            {
                return configApp.Note;
            }
            return "";
        }

        public int GetGapTimeSuggest()
        {
            ConfigApp? configApp = _dbSet.FirstOrDefault(con => con.Name.Equals(ConfigConstant.GAP_TIME_SUGGEST));
            if (configApp != null)
            {
                return int.Parse(configApp.Note);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }
    }
}
