using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.ReportModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Services.PackageService
{
    public interface IPackageServiceScript
    {
        Task<ApiResponse<ResponsePackageModel>> Create(CreatePackageModel model);
        Task<ApiResponse<ResponsePackageModel>> GetById(Guid id);
        Task<ApiResponsePaginated<ResponsePackageModel>> GetFilter(Guid? deliverId, Guid? senderId, string? id, string? status, int pageIndex, int pageSize);
        Task<ApiResponse<List<ResponsePackageModel>>> GetAll(Guid deliverId, Guid senderId, string? status);
        Task<ApiResponse> ApprovedPackage(Package package, bool isNotify = false);
        Task<ApiResponse> RejectPackage(Package package);
        Task<ApiResponse> DeliverSelectedPackages(Guid deliverId, List<Package> package);
        Task<ApiResponse> DeliverSelectedPackage(Guid deliverId, Package package);
        Task<ApiResponse> SenderCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> DeliverCancelPackage(Guid packageId, string? reason);
        Task<ApiResponse> PickupPackageFailed(PickupPackageFailedModel model);
        Task<ApiResponse> PickupPackageSuccess(Package package);
        Task<ApiResponse> DeliveredSuccess(Package package);
        Task<ApiResponse> ToSuccessPackage(Guid packageId);
        Task<ApiResponse> DeliveredFailed(DeliveredFailedModel packageId);
        Task<ApiResponse> ReportProblem(CreateReportPackageModel model);
        Task<ApiResponse> RefundToWarehouseSuccess(Package package);
        Task<ApiResponse> RefundToWarehouseFailed(Guid packageId);
       
    }
}
