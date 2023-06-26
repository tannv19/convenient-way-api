using ship_convenient.Core.CoreModel;
using ship_convenient.Model.RouteModel;

namespace ship_convenient.Services.RouteService
{
    public interface IRouteService
    {
        Task<ApiResponse<List<ResponseRouteModel>>> GetRouteUserId(Guid infoUserId);
        Task<ApiResponse> SetActiveRoute(Guid routeId);
        Task<ApiResponse<ResponseRouteModel>> Create(CreateRouteModel model);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse<ResponseRoutePointListModel>> GetPointList(Guid routeId);
        Task<ApiResponse<List<ResponseRoutePointModel>>> GetPointListVirtual(Guid accountId);
    }
}
