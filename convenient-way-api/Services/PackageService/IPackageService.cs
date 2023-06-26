using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.ReportModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Services.PackageService
{
    public interface IPackageService
    {
        Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model);
        Task<ApiResponse<ResponsePackageModel>> GetById(Guid id);
        Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? senderId,string? id, string? status, int pageIndex, int pageSize);
        Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid senderId, string? status);
        Task<ApiResponse> ApprovedPackage(Guid id, bool isNotify = true);
        Task<ApiResponse> RejectPackage(Guid id);
        Task<ApiResponse> DeliverSelectedPackages(Guid deliverId, List<Guid> packageIds, bool isScript = false);
        Task<ApiResponse> SenderCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> DeliverCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> PickupPackageFailed(PickupPackageFailedModel model, bool isScript = false);
        Task<ApiResponse> PickupPackageSuccess(Guid packageId);
        Task<ApiResponse> DeliveredSuccess(Guid packageId, bool isScript = false); 
        Task<ApiResponse> ToSuccessPackage(Guid packageId);
        Task<ApiResponse> DeliveredFailed(DeliveredFailedModel packageId, bool isScript = false);
        Task<ApiResponse> ReportProblem(CreateReportPackageModel model);
        Task<ApiResponse> RefundToWarehouseSuccess(Guid packageId);
        Task<ApiResponse> RefundToWarehouseFailed(Guid packageId);
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestCombo(Guid deliverId);
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestComboV2(Guid deliverId);
        Task<ApiResponse<List<ResponseComboPackageModel>>> SuggestComboV3(Guid deliverId);
        Task<List<Package>> GetPackagesNearTimePickup();
        Task<List<Package>> GetPackagesNearTimeDelivery();
    }
}
