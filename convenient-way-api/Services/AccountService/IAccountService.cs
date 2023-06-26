using ship_convenient.Core.CoreModel;
using ship_convenient.Helper.SuggestPackageHelper;
using ship_convenient.Model.GoongModel;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Services.AccountService
{
    public interface IAccountService
    {
        Task<ApiResponse<ResponseAccountModel>> Create(CreateAccountModel model);
        Task<ApiResponse> IsCanCreate(VerifyValidAccountModel model);
        Task<ApiResponse<ResponseAccountModel>> Update(UpdateAccountModel model);
        Task<ApiResponse<ResponseAccountModel>> UpdateInfo(UpdateInfoModel model);
        Task<ApiResponse<ResponseAccountModel>> GetId(Guid id);
        Task<ApiResponse> UpdateRegistrationToken(UpdateTokenModel model);
        Task<ApiResponsePaginated<ResponseAccountModel>> GetList(string? userName, string? status,string? role, int pageIndex, int pageSize);
        Task<ApiResponse<ResponseBalanceModel>> AvailableBalance(Guid accountId);
        Task<ApiResponse<List<DistancePackageModel>>> GetOrderPointVirtual(Guid accountId);

        Task<ApiResponse<List<ResponseSearchModel>>> GetHistoryLocation(Guid senderId);
    }
}
