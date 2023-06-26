using ship_convenient.Core.CoreModel;
using ship_convenient.Model.ConfigUserModel;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Services.ConfigUserService
{
    public interface IConfigUserService
    {
        Task<ApiResponse<List<ResponseConfigUserModel>>> GetList(Guid accountId);
        Task<ApiResponse<ResponseConfigUserModel>> Update(UpdateUserConfigModel model);
        Task<ApiResponse> UpdateLocationUser(UpdateLocationUserModel model);
    }
}
