using ship_convenient.Entities;

namespace ship_convenient.Core.IRepository
{
    public interface IConfigUserRepository : IGenericRepository<ConfigUser>
    {
        string GetValueConfig(string configName, Guid infoId);
        int GetPackageDistance(Guid infoId);
        string GetDirectionSuggest(Guid infoId);
        int GetWarningPrice(Guid infoId);
   
    }
}
