using ship_convenient.Core.CoreModel;

namespace ship_convenient.Services.ScriptService
{
    public interface IScriptService
    {
        Task<ApiResponse> CreateActiveAccount(int deliverCount, int senderCount);
        Task<ApiResponse> CreatePackages(int packageCount);
        Task<ApiResponse> ApprovedPackages(int packageCountApproved, int packageCountReject);
        Task<ApiResponse> SelectedPackages(int selectedSuccess);
        Task<ApiResponse> PickupPackages(int pickupSuccess, int pickupFailed);
        Task<ApiResponse> DeliveredPackages(int deliveredSuccess, int deliveredFailed);
        Task<ApiResponse> CompletePackegs(int completeSuccess);
        Task<ApiResponse> RemoveScriptData();
    }
}
