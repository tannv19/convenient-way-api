using ship_convenient.Core.CoreModel;
using ship_convenient.Model.DashboardModel;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ApiResponse<List<ResponseAccountModel>>> GetListAccountActive();
        Task<ApiResponse<PackageCountModel>> GetCountPackage();
        Task<ApiResponse<PackageCountModel>> GetCountPackage(Guid? deliverId, Guid? senderId);
    }
}
