using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Helper.SuggestPackageHelper;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.GenericService;
using ship_convenient.Services.GoongService;
using ship_convenient.Services.MapboxService;
using System.Linq.Expressions;
using unitofwork_core.Constant.Package;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Services.PackageService
{
    public class PackageSuggestService : GenericService<PackageSuggestService>
    {
        private readonly IMapboxService _mapboxService;
        private readonly IGoongService _goongService;
        private readonly AccountUtils _accountUtils;
        public PackageSuggestService(ILogger<PackageSuggestService> logger, IUnitOfWork unitOfWork,
            IMapboxService mapboxService,IGoongService goongService, AccountUtils accountUtils) : base(logger, unitOfWork)
        {
            _mapboxService = mapboxService;
            _accountUtils = accountUtils;
            _goongService = goongService;
        }

        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestCombo(Guid deliverId)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId
                , include: (source) => source.Include(acc => acc.InfoUser));
            if (deliver == null)
            {
                response.ToFailedResponse("Người dùng không tồn tại");
                return response;
            }
            if (deliver.InfoUser == null)
            {
                response.ToFailedResponse("Thông tin người dùng chưa được tạo");
                return response;
            }
            List<string> statusNotComplete = new List<string> {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS
            };
            Expression<Func<Package, bool>> predicate = pa => pa.DeliverId == deliverId && statusNotComplete.Contains(pa.Status);
            List<Package> packagesNotComplete = await _packageRepo.GetAllAsync(predicate: predicate);
            if (packagesNotComplete.Count == 0)
            {
                response = await SuggestComboFirst(deliverId);
            }
            else if (packagesNotComplete.Count == 1)
            {
                response = await SuggestComboSecond(deliverId, packagesNotComplete[0]);
            }
            else
            {
                response.ToFailedResponse("Bạn đã nhận quá số lượng đơn hàng");
            }
            return response;
        }

        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestComboFirst(Guid deliverId)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            #region Verify params
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId
                , include: (source) => source.Include(acc => acc.InfoUser));
            if (deliver == null)
            {
                response.ToFailedResponse("Người dùng không tồn tại");
                return response;
            }
            if (deliver.InfoUser == null)
            {
                response.ToFailedResponse("Thông tin người dùng chưa được tạo");
                return response;
            }
            Route? route = await _routeRepo.FirstOrDefaultAsync(
                    predicate: (rou) => rou.InfoUserId == deliver!.InfoUser!.Id && rou.IsActive == true);
            int spacingValid = _configUserRepo.GetPackageDistance(deliver.InfoUser.Id);
            string directionSuggest = _configUserRepo.GetDirectionSuggest(deliver.InfoUser.Id);
            #endregion
            #region Get route points deliver
            List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id);
            if (directionSuggest == DirectionTypeConstant.FORWARD)
            {
                routePoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
            {
                routePoints = routePoints.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            #endregion
            #region Includale package
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> include = (source) => source.Include(p => p.Products);
            #endregion
            #region Predicate package
            Expression<Func<Package, bool>> predicate = (source) => source.Status == PackageStatus.APPROVED && source.SenderId != deliverId;
            #endregion

            #region Find packages valid spacing
            List<ResponseSuggestPackageModel> packagesValid;
            if (route == null)
            {
                packagesValid = _packageRepo.GetAllAsync(include: include, predicate: predicate).Result
                    .Select(p => p.ToResponseSuggestModel()).ToList();
            }
            else
            {
                packagesValid = new();
                List<Package> packages = (await _packageRepo.GetAllAsync(include: include, predicate: predicate)).ToList();
                int packageCount = packages.Count;
                for (int i = 0; i < packageCount; i++)
                {
                    bool isValidRouteAndDirection = MapHelper.IsTrueWithPackageAndUserRoute(
                        directionSuggest, routePoints, route, packages[i], spacingValid * 0.6);
                    if (isValidRouteAndDirection)
                    {
                        List<GeoCoordinate> listPoints = MapHelper.GetListPointOrder(directionSuggest, packages[i], route);
                        DirectionApiModel requestModel = DirectionApiModel.FromListGeoCoordinate(listPoints);
                        List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        if (listPolyline.Count > 0)
                        {
                            if (directionSuggest == DirectionTypeConstant.FORWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward;
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceBackward;
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.TWO_WAY)
                            {
                                {
                                    bool isMaxSpacingError = listPolyline[0].Distance > (spacingValid + route.DistanceForward + route.DistanceBackward);
                                    if (!isMaxSpacingError)
                                    {
                                        ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                        suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                        packagesValid.Add(suggest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Valid package with balance
            int balanceAvailable = await _accountUtils.AvailableBalanceAsync(deliverId);
            packagesValid = packagesValid
                .Where(pa => balanceAvailable - pa.GetPriceProducts() >= 0).ToList();
            #endregion
            int maxSuggestCombo = _configRepo.GetMaxSuggestCombo();
            List<ResponseSuggestPackageModel> result = packagesValid.Take(maxSuggestCombo).ToList();
            response.ToSuccessResponse(result, "Lấy đề xuất thành công");

            return response;
        }

        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestComboSecond(Guid deliverId, Package oldPackage)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            #region Verify params
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId
                , include: (source) => source.Include(acc => acc.InfoUser));
            if (deliver == null)
            {
                response.ToFailedResponse("Người dùng không tồn tại");
                return response;
            }
            if (deliver.InfoUser == null)
            {
                response.ToFailedResponse("Thông tin người dùng chưa được tạo");
                return response;
            }
            Route? route = await _routeRepo.FirstOrDefaultAsync(
                    predicate: (rou) => rou.InfoUserId == deliver!.InfoUser!.Id && rou.IsActive == true);
            int spacingValid = _configUserRepo.GetPackageDistance(deliver.InfoUser.Id);
            string directionSuggest = _configUserRepo.GetDirectionSuggest(deliver.InfoUser.Id);
            #endregion
            #region Get route points deliver
            List<RoutePoint> routePointsOrigin = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == false);
            List<RoutePoint> routePointVirtual = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == true);
            if (directionSuggest == DirectionTypeConstant.FORWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            #endregion
            #region Includale package
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> include = (source) => source.Include(p => p.Products);
            #endregion
            #region Predicate package
            Expression<Func<Package, bool>> predicate = (source) => source.Status == PackageStatus.APPROVED && source.SenderId != deliverId;
            #endregion

            #region Find packages valid spacing
            List<ResponseSuggestPackageModel> packagesValid;
            if (route == null)
            {
                packagesValid = _packageRepo.GetAllAsync(include: include, predicate: predicate).Result
                    .Select(p => p.ToResponseSuggestModel()).ToList();
            }
            else
            {
                packagesValid = new();
                List<Package> packages = (await _packageRepo.GetAllAsync(include: include, predicate: predicate)).ToList();
                int packageCount = packages.Count;
                for (int i = 0; i < packageCount; i++)
                {
                    bool isValidRouteAndDirection = MapHelper.IsTrueWithPackageAndUserRoute(
                        directionSuggest, routePointsOrigin, route, packages[i], spacingValid * 0.6);
                    if (isValidRouteAndDirection)
                    {
                        List<GeoCoordinate> listPoints = MapHelper.GetListPointOrderSecond(directionSuggest, oldPackage, packages[i], route);
                        DirectionApiModel requestModel = DirectionApiModel.FromListGeoCoordinate(listPoints);
                        List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        if (listPolyline.Count > 0)
                        {
                            if (directionSuggest == DirectionTypeConstant.FORWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward;
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceBackward;
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.TWO_WAY)
                            {
                                {
                                    bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward + route.DistanceBackward;
                                    if (!isMaxSpacingError)
                                    {
                                        ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                        suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                        packagesValid.Add(suggest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Valid package with balance
            int balanceAvailable = await _accountUtils.AvailableBalanceAsync(deliverId);
            packagesValid = packagesValid
                .Where(pa => balanceAvailable - pa.GetPriceProducts() >= 0).ToList();
            #endregion
            int maxSuggestCombo = _configRepo.GetMaxSuggestCombo();
            List<ResponseSuggestPackageModel> result = packagesValid.Take(maxSuggestCombo).ToList();
            response.ToSuccessResponse(result, "Lấy đề xuất thành công");

            return response;
        }


        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestComboV2(Guid deliverId)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            Account? deliver = await _accountRepo.GetByIdAsync(deliverId
                , include: (source) => source.Include(acc => acc.InfoUser));
            if (deliver == null)
            {
                response.ToFailedResponse("Người dùng không tồn tại");
                return response;
            }
            if (deliver.InfoUser == null)
            {
                response.ToFailedResponse("Thông tin người dùng chưa được tạo");
                return response;
            }
            List<string> statusNotComplete = new List<string> {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS
            };
            Expression<Func<Package, bool>> predicate = pa => pa.DeliverId == deliverId && statusNotComplete.Contains(pa.Status);
            List<Package> packagesNotComplete = await _packageRepo.GetAllAsync(predicate: predicate);
            response = await SuggestComboLogicV3Log(deliver, packagesNotComplete);
            return response;
        }


        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestComboLogicV2(Account deliver, List<Package> packagesNotComplete)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            #region Verify params
            Route? route = await _routeRepo.FirstOrDefaultAsync(
                    predicate: (rou) => rou.InfoUserId == deliver!.InfoUser!.Id && rou.IsActive == true);
            int spacingValid = _configUserRepo.GetPackageDistance(deliver.InfoUser.Id);
            string directionSuggest = _configUserRepo.GetDirectionSuggest(deliver.InfoUser.Id);
            #endregion
            #region Get route points deliver
            List<RoutePoint> routePointsOrigin = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == false);
            List<RoutePoint> routePointVirtual = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == true);
            if (directionSuggest == DirectionTypeConstant.FORWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            #endregion
            #region Includale package
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> include = (source) => source.Include(p => p.Products);
            #endregion
            #region Predicate package
            Expression<Func<Package, bool>> predicate = (source) => source.Status == PackageStatus.APPROVED && source.SenderId != deliver.Id;
            #endregion

            #region Find packages valid spacing
            List<ResponseSuggestPackageModel> packagesValid;
            if (route == null)
            {
                packagesValid = _packageRepo.GetAllAsync(include: include, predicate: predicate).Result
                    .Select(p => p.ToResponseSuggestModel()).ToList();
            }
            else
            {
                packagesValid = new();
                List<Package> packages = (await _packageRepo.GetAllAsync(include: include, predicate: predicate)).ToList();
                int packageCount = packages.Count;

                _logger.LogInformation($"AccountId : {deliver.Id}" +
                    $"          Số lượng gói hàng đã được duyệt: {packageCount}" +
                    $"          Độ dài lộ trình gốc: {Math.Round(route.GetDistanceSuggest(suggestDirection: directionSuggest) / 1000,2)}km" +
                    $"          Điểm đi: {route.FromName}\nĐiểm đến: {route.ToName}" +
                    $"          Khoảng cách tối đa cho phép: {spacingValid / 1000}km");
                
                for (int i = 0; i < packageCount; i++)
                {
                    _logger.LogInformation($"{i}. =============================");
                    _logger.LogInformation($"Mã gói hàng: {packages[i].Id}");
                    _logger.LogInformation($"Điểm lấy hàng: {packages[i].StartAddress}");
                    _logger.LogInformation($"Điểm giao hàng: {packages[i].DestinationAddress}");

                    bool isValidRouteAndDirection = MapHelper.IsTrueWithPackageAndUserRoute(
                        directionSuggest, routePointsOrigin, route, packages[i], spacingValid * 0.6);
                    _logger.LogInformation($"Kiểm tra hướng và khoảng cách của gói hàng đường chim bay: {isValidRouteAndDirection}");
                    if (isValidRouteAndDirection)
                    {
                        List<Package> allPackageWillOrder = new List<Package>(packagesNotComplete);
                        allPackageWillOrder.Add(packages[i]);
                        List<GeoCoordinate> listPoints = SuggestPackageHelper.GetListPointOrder(directionSuggest, allPackageWillOrder, route);
                        DirectionApiModel requestModel = DirectionApiModel.FromListGeoCoordinate(listPoints);
                        List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        _logger.LogInformation($"Độ dài lộ trình thực tế: {string.Format("{0:F2}", listPolyline[0].Distance/1000)}km");
                        if (listPolyline.Count > 0)
                        {
                            if (directionSuggest == DirectionTypeConstant.FORWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward;
                                _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceBackward;
                                _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.TWO_WAY)
                            {
                                {
                                    bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward + route.DistanceBackward;
                                    _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                    if (!isMaxSpacingError)
                                    {
                                        ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                        suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                        packagesValid.Add(suggest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Valid package with balance
            int balanceAvailable = await _accountUtils.AvailableBalanceAsync(deliver.Id);
            packagesValid = packagesValid
                .Where(pa => balanceAvailable - pa.GetPriceProducts() >= 0).ToList();
            #endregion
            int maxSuggestCombo = _configRepo.GetMaxSuggestCombo();
            List<ResponseSuggestPackageModel> result = packagesValid.Take(maxSuggestCombo).ToList();
            response.ToSuccessResponse(result, "Lấy đề xuất thành công");

            return response;
        }

        public async Task<ApiResponse<List<ResponseSuggestPackageModel>>> SuggestComboLogicV3Log(Account deliver, List<Package> packagesNotComplete)
        {
            ApiResponse<List<ResponseSuggestPackageModel>> response = new();
            #region Verify params
            Route? route = await _routeRepo.FirstOrDefaultAsync(
                    predicate: (rou) => rou.InfoUserId == deliver!.InfoUser!.Id && rou.IsActive == true);
            int spacingValid = _configUserRepo.GetPackageDistance(deliver.InfoUser.Id);
            string directionSuggest = _configUserRepo.GetDirectionSuggest(deliver.InfoUser.Id);
            #endregion
            #region Get route points deliver
            List<RoutePoint> routePointsOrigin = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == false);
            List<RoutePoint> routePointVirtual = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => route == null ? false : routePoint.RouteId == route.Id && routePoint.IsVitual == true);
            if (directionSuggest == DirectionTypeConstant.FORWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
            {
                routePointsOrigin = routePointsOrigin.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            }
            double? distanceSuggest;
            if (directionSuggest == DirectionTypeConstant.FORWARD)
            {
                distanceSuggest = route?.DistanceForward;
            } else if (directionSuggest == DirectionTypeConstant.BACKWARD) {
                distanceSuggest = route?.DistanceBackward;
            } else {
                distanceSuggest = route?.DistanceForward + route.DistanceBackward;
            }
            
            #endregion
            #region Includale package
            Func<IQueryable<Package>, IIncludableQueryable<Package, object>> include = (source) => source.Include(p => p.Products);
            #endregion
            #region Predicate package
            Expression<Func<Package, bool>> predicate = (source) => source.Status == PackageStatus.APPROVED && source.SenderId != deliver.Id;
            #endregion

            #region Find packages valid spacing
            List<ResponseSuggestPackageModel> packagesValid;
            if (route == null)
            {
                packagesValid = _packageRepo.GetAllAsync(include: include, predicate: predicate).Result
                    .Select(p => p.ToResponseSuggestModel()).ToList();
            }
            else
            {
                packagesValid = new();
                List<Package> packages = (await _packageRepo.GetAllAsync(include: include, predicate: predicate)).ToList();
                int packageCount = packages.Count;

                _logger.LogInformation($"Số lượng gói hàng đã được duyệt: {packageCount}" +
                    $"      Độ dài lộ trình gốc: {Math.Round(route.GetDistanceSuggest(suggestDirection: directionSuggest) / 1000, 2)}km" +
                    $"      Điểm đi: {route.FromName}\nĐiểm đến: {route.ToName}" +
                    $"      Khoảng cách tối đa cho phép: {spacingValid / 1000}km");

                for (int i = 0; i < packageCount; i++)
                {
                    _logger.LogInformation($"{i}. =============================");
                    _logger.LogInformation($"Mã gói hàng: {packages[i].Id}");
                    _logger.LogInformation($"Điểm lấy hàng: {packages[i].StartAddress}");
                    _logger.LogInformation($"Điểm giao hàng: {packages[i].DestinationAddress}");

                    PackageDistanceValid packageDistanceValid = MapHelper.IsTrueWithPackageAndUserRouteV2(
                        directionSuggest, routePointsOrigin, route, packages[i], spacingValid * 0.6);
                    string textResultRoute = packageDistanceValid.IsValid ? "Hợp lệ" : "Không hợp lệ";
                    _logger.LogInformation($"Kiểm tra hướng và khoảng cách của gói hàng đường chim bay: {textResultRoute}");
                    _logger.LogInformation($"Khoảng cách điểm bắt đầu của gói hàng với lộ trình: {Math.Round(packageDistanceValid.StartPointMinDistance / 1000, 2)}km");
                    _logger.LogInformation($"Khoảng cách điểm kết thúc của gói hàng với lộ trình: {Math.Round(packageDistanceValid.EndPointMinDistance / 1000, 2)}km");
                    if (packageDistanceValid.IsValid)
                    {
                        List<Package> allPackageWillOrder = new List<Package>(packagesNotComplete);
                        allPackageWillOrder.Add(packages[i]);
                        List<GeoCoordinate> listPoints = SuggestPackageHelper.GetListPointOrder(directionSuggest, allPackageWillOrder, route);
                        DirectionApiModel requestModel = DirectionApiModel.FromListGeoCoordinate(listPoints);
                        // List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        _logger.LogInformation($"Độ dài lộ trình thực tế: {string.Format("{0:F2}", listPolyline[0].Distance / 1000)}km (Chênh lệch  {string.Format("{0:F2}", listPolyline[0].Distance / 1000 - distanceSuggest / 1000)}km)");
           
                        if (directionSuggest != null) {
                            _logger.LogInformation($"Chênh lệch  {string.Format("{0:F2}", listPolyline[0].Distance / 1000 - distanceSuggest / 1000)}km");
                        }
                        if (listPolyline.Count > 0)
                        {
                            if (directionSuggest == DirectionTypeConstant.FORWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward;
                                _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceBackward;
                                _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                if (!isMaxSpacingError)
                                {
                                    ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                    suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                    packagesValid.Add(suggest);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.TWO_WAY)
                            {
                                {
                                    bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + route.DistanceForward + route.DistanceBackward;
                                    _logger.LogInformation(isMaxSpacingError ? "Gói hàng không hợp lệ" : "Gói hàng hợp lệ");
                                    if (!isMaxSpacingError)
                                    {
                                        ResponseSuggestPackageModel suggest = packages[i].ToResponseSuggestModel();
                                        suggest.DistanceExtend = listPolyline[0].Distance ?? 0;
                                        packagesValid.Add(suggest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Valid package with balance
            int balanceAvailable = await _accountUtils.AvailableBalanceAsync(deliver.Id);
            packagesValid = packagesValid
                .Where(pa => balanceAvailable - pa.GetPriceProducts() >= 0).ToList();
            #endregion
            int maxSuggestCombo = _configRepo.GetMaxSuggestCombo();
            List<ResponseSuggestPackageModel> result = packagesValid.Take(maxSuggestCombo).ToList();
            response.ToSuccessResponse(result, "Lấy đề xuất thành công");

            return response;
        }
    }
}