using ship_convenient.Entities;

namespace ship_convenient.Core.IRepository
{
    public interface IConfigRepository : IGenericRepository<ConfigApp>
    {
        string GetValueConfig(string configName);
        int GetProfitPercentage();
        int GetProfitPercentageRefund();
        int GetMinimumDistance();
        int GetMaxPickupSameTime();
        int GetMaxRouteCreate();
        int GetDefaultBalanceNewAccount();
        int GetMaxSuggestCombo();
        int GetMaxCancelInDay();
        int GetGapTimeSuggest();
    }
}
