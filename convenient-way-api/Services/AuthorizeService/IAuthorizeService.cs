using ship_convenient.Core.CoreModel;
using ship_convenient.Model.AuthorizeModel;

namespace ship_convenient.Services.AuthorizeService
{
    public interface IAuthorizeService
    {
        Task<ApiResponse<ResponseLoginModel>> Login(LoginModel model);
        Task<ApiResponse<ResponseLoginModel>> Login(LoginFirebaseModel model);
        Task<ApiResponse> LogOut(Guid accountId);
    }
}
