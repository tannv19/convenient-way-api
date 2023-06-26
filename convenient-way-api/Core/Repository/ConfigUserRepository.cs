using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class ConfigUserRepository : GenericRepository<ConfigUser>, IConfigUserRepository
    {
        public ConfigUserRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public string GetDirectionSuggest(Guid infoId)
        {
            ConfigUser? configUser = _dbSet.FirstOrDefault(con => con.Name.Equals(DefaultUserConfigConstant.DIRECTION_SUGGEST)
                && con.InfoUserId.Equals(infoId));
            if (configUser != null)
            {
                return configUser.Value;
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetPackageDistance(Guid infoId)
        {
            ConfigUser? configUser = _dbSet.FirstOrDefault(con => con.Name.Equals(DefaultUserConfigConstant.PACKAGE_DISTANCE)
              && con.InfoUserId.Equals(infoId));
            if (configUser != null)
            {
                return int.Parse(configUser.Value);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }
        
        public string GetValueConfig(string configName, Guid infoId)
        {
            ConfigUser? configUser = _dbSet.FirstOrDefault(con => con.Name.Equals(configName) && con.InfoUserId.Equals(infoId));
            if (configUser != null)
            {
                return configUser.Value;
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }

        public int GetWarningPrice(Guid infoId)
        {
            ConfigUser? configUser = _dbSet.FirstOrDefault(con => con.Name.Equals(DefaultUserConfigConstant.WARNING_PRICE)
                && con.InfoUserId.Equals(infoId));
            if (configUser != null)
            {
                return int.Parse(configUser.Value);
            }
            throw new ArgumentNullException("Không tìm thấy thông tin cấu hình");
        }
    }
}
