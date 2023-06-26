using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Model.RouteModel;
using Route = ship_convenient.Entities.Route;
using ship_convenient.Services.GenericService;
using ship_convenient.Entities;
using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Services.AccountService;
using System.Linq;

namespace ship_convenient.Services.RouteService
{
    public class RouteService : GenericService<RouteService>, IRouteService
    {
        private readonly IInfoUserRepository _infoUserRepo;
        private readonly AccountUtils _accountUtils;
        public RouteService(ILogger<RouteService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils) : base(logger, unitOfWork)
        {
            _infoUserRepo = unitOfWork.InfoUsers;
            _accountUtils = accountUtils;
        }

        public async Task<ApiResponse<ResponseRouteModel>> Create(CreateRouteModel model)
        {
            ApiResponse<ResponseRouteModel> response = new ApiResponse<ResponseRouteModel>();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (i) => i.Id == model.AccountId, disableTracking: false, include: (en) => en.Include(info => info.InfoUser));

            #region verify params
            if (account == null)
            {
                response.ToFailedResponse("Thông tin người dùng không tồn tại");
                return response;
            }
            #endregion
            Route route = model.ConvertToEntity(account.InfoUser!.Id);
            if (account.Status == AccountStatus.NO_ROUTE)
            {
                route.IsActive = true;
                await _routeRepo.InsertAsync(route);
                account.Status = AccountStatus.ACTIVE;
            }
            else
            {
                route.IsActive = false;
                await _routeRepo.InsertAsync(route);
            }
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(route.ToResponseModel(), "Tạo tuyến đường thành công");
            }
            else
            {
                response.ToFailedResponse("Tạo tuyến đường thất bại");
            }
            return response;

        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            ApiResponse reponse = new();
            Route? route = await _routeRepo.GetByIdAsync(id);
            if (route != null)
            {
                await _routeRepo.DeleteAsync(id);
                int result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    reponse.ToSuccessResponse("Xóa tuyến đường thành công");
                    return reponse;
                }
            }
            reponse.ToFailedResponse("Xóa tuyến đường thất bại");
            return reponse;
        }

        public async Task<ApiResponse<ResponseRoutePointListModel>> GetPointList(Guid routeId)
        {
            ApiResponse<ResponseRoutePointListModel> response = new();
            List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate: (routePoint) => routePoint.RouteId == routeId && routePoint.IsVitual == false);
            List<ResponseRoutePointModel> forwardPoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                .OrderBy(source => source.Index).Select(source => source.ToResponseModel()).ToList();
            List<ResponseRoutePointModel> backwardPoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                .OrderBy(source => source.Index).Select(source => source.ToResponseModel()).ToList();
            ResponseRoutePointListModel data = new();
            data.RouteId = routeId;
            data.ForwardPoints = forwardPoints;
            data.BackwardPoints = backwardPoints;
            response.ToSuccessResponse(data, "Lấy danh sách điểm thành công");
            return response;
        }

        public async Task<ApiResponse<List<ResponseRoutePointModel>>> GetPointListVirtual(Guid accountId)
        {
            ApiResponse<List<ResponseRoutePointModel>> response = new();
            Route? activeRoute = await _accountUtils.GetActiveRoute(accountId);
            List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate: (routePoint) => routePoint.RouteId == activeRoute.Id && routePoint.IsVitual == true, orderBy: source => source.OrderBy(point => point.Index));
            List<ResponseRoutePointModel> virtualRoute = routePoints.Select(route => route.ToResponseModel()).ToList();
            response.ToSuccessResponse(virtualRoute, "Lấy danh sách điểm thành công");
            return response;
        }

        public async Task<ApiResponse<List<ResponseRouteModel>>> GetRouteUserId(Guid accountId)
        {
            ApiResponse<List<ResponseRouteModel>> response = new();
            List<ResponseRouteModel> routes = _routeRepo.GetAll(predicate: (route) =>
                route.InfoUser != null ? route.InfoUser.AccountId == accountId : false,
                selector: route => route.ToResponseModel(),
                orderBy: source => source.OrderByDescending(route => route.IsActive)).ToList();
            if (routes.Count > 0)
            {
                response.ToSuccessResponse(routes, "Lấy thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Bạn chưa tạo tuyến đường nào");
            }
            return await Task.FromResult(response);

        }

        public async Task<ApiResponse> SetActiveRoute(Guid routeId)
        {
            ApiResponse response = new ApiResponse();
            Route? route = await _routeRepo.GetByIdAsync(routeId);
            if (route != null)
            {
                List<Route> routes = await _routeRepo.GetAllAsync(predicate:
                    routeFilter => route.InfoUserId == routeFilter.InfoUserId, disableTracking: false);
                for (int i = 0; i < routes.Count; i++)
                {
                    if (routes[i].Id == route.Id)
                    {
                        routes[i].IsActive = true;
                    }
                    else
                    {
                        routes[i].IsActive = false;
                    }
                }
                int result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    response.ToSuccessResponse("Yêu cầu thành công");
                    return response;

                }
            }
            response.ToFailedResponse("Yêu cầu không thành công");
            return response;
        }

       
    }
}
