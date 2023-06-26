using ship_convenient.Core.CoreModel;
using ship_convenient.Model.DepositModel;

namespace ship_convenient.Services.DepositService
{
    public interface IDepositService
    {
        Task<ApiResponse<ResponseDepositModel>> Create(CreateDepositModel model);
        Task<ApiResponse<ResponseDepositModel>> GetId(Guid id);
        
    }
}
