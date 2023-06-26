using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Helper.SuggestPackageHelper;
using ship_convenient.Model.FirebaseNotificationModel;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.GenericService;
using ship_convenient.Services.GoongService;
using ship_convenient.Services.MapboxService;
using System.Linq.Expressions;
using unitofwork_core.Constant.ConfigConstant;
using unitofwork_core.Constant.Package;
using unitofwork_core.Model.PackageModel;
using unitofwork_core.Model.ProductModel;
using RouteEntity = ship_convenient.Entities.Route;

namespace ship_convenient.Services.PackageService
{
    public class PackageUtils : GenericService<PackageService>
    {
        private readonly AccountUtils _accountUtils;
        protected readonly IFirebaseCloudMsgService _fcmService;
        private readonly IMapboxService _mapboxService;
        private readonly IGoongService _goongService;

        public PackageUtils(ILogger<PackageService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils, IMapboxService mapboxService
            , IFirebaseCloudMsgService fcmService, IGoongService goongService) : base(logger, unitOfWork)
        {
            _accountUtils = accountUtils;
            _fcmService = fcmService;
            _mapboxService = mapboxService;
            _goongService = goongService;
        }

        public bool IsValidComboBalance(int accountBalance, ResponseComboPackageModel combo)
        {
            bool result = false;
            if (accountBalance - combo.ComboPrice >= 0) result = true;
            return result;
        }

        public async Task<bool> IsMaxCancelInDay(Guid deliverId)
        {
            bool result = false;
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.DeliverId == deliverId && p.Status == PackageStatus.DELIVER_CANCEL);
            packages = packages.Where(p => Utils.IsTimeToday(p.ModifiedAt)).ToList();
            if (packages.Count > _configRepo.GetMaxCancelInDay()) result = true;
            return result;
        }


        public async Task NotificationValidUserWithPackage(Package package)
        {
            List<Account> activeAccounts = await _accountUtils.GetListAccountActive();
            int pricePackage = package.GetPricePackage();
            List<Account> validBalanceAccounts = activeAccounts.Where(ac => (_accountUtils.AvailableBalance(ac.Id) - pricePackage) > 0).ToList();
            int count = validBalanceAccounts.Count;
            for (int i = 0; i < count; i++)
            {
                Account account = validBalanceAccounts[i];
                int spacingValid = _configUserRepo.GetPackageDistance(account.InfoUser.Id);
                string directionSuggest = _configUserRepo.GetDirectionSuggest(account.InfoUser.Id);
                RouteEntity? activeRoute = await _routeRepo.FirstOrDefaultAsync(predicate: route =>
                        route.InfoUserId == account.InfoUser.Id && route.IsActive == true);
                if (activeRoute != null)
                {
                    List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate:
                            (routePoint) => activeRoute == null ? false : routePoint.RouteId == activeRoute.Id);
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
                    bool isValidOrder = MapHelper.ValidDestinationBetweenDeliverAndPackage(routePoints, package, spacingValid);
                    if (isValidOrder && account.RegistrationToken != "")
                    {

                        SendNotificationModel model = new SendNotificationModel()
                        {
                            AccountId = account.Id,
                            Title = "Có gói hàng phù hợp",
                            Body = "Có gói hàng phù hợp với lộ trình của bạn, nhanh tay vào và nhận thôi!!!"
                        };
                        try
                        {
                            await _fcmService.SendNotification(model);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("FCM exception" + ex.Message);
                        }
                    }
                }


            }
        }

        public async Task NotificationValidUserWithPackageV2(Package package)
        {
            List<Account> activeAccounts = await _accountUtils.GetListAccountActive();
            activeAccounts = activeAccounts.Where(acc => !acc.UserName.Contains("[script]")).ToList();
            int pricePackage = package.GetPricePackage();
            List<Account> validBalanceAccounts = activeAccounts.Where(ac => (_accountUtils.AvailableBalance(ac.Id) - pricePackage) > 0).ToList();
            int count = validBalanceAccounts.Count;
            for (int i = 0; i < count; i++)
            {
                Account account = validBalanceAccounts[i];
                int spacingValid = _configUserRepo.GetPackageDistance(account.InfoUser.Id);
                string directionSuggest = _configUserRepo.GetDirectionSuggest(account.InfoUser.Id);
                RouteEntity? activeRoute = await _routeRepo.FirstOrDefaultAsync(predicate: route =>
                        route.InfoUserId == account.InfoUser.Id && route.IsActive == true);
                List<string> statusNotComplete = new List<string> {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS
                    };
                Expression<Func<Package, bool>> predicate = pa => pa.DeliverId == account.Id && statusNotComplete.Contains(pa.Status);
                List<Package> packagesNotComplete = await _packageRepo.GetAllAsync(predicate: predicate);
                if (activeRoute != null)
                {
                    List<RoutePoint> routePointsOrigin = await _routePointRepo.GetAllAsync(predicate:
                (routePoint) => activeRoute == null ? false : routePoint.RouteId == activeRoute.Id && routePoint.IsVitual == false);
                    List<RoutePoint> routePointVirtual = await _routePointRepo.GetAllAsync(predicate:
                        (routePoint) => activeRoute == null ? false : routePoint.RouteId == activeRoute.Id && routePoint.IsVitual == true);
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
                    bool isValidRouteAndDirection = MapHelper.IsTrueWithPackageAndUserRoute(
                       directionSuggest, routePointsOrigin, activeRoute, package, spacingValid * 0.6);

                    if (isValidRouteAndDirection && account.RegistrationToken != "")
                    {
                        List<Package> allPackageWillOrder = new List<Package>(packagesNotComplete);
                        allPackageWillOrder.Add(package);
                        List<GeoCoordinate> listPoints = SuggestPackageHelper.GetListPointOrder(directionSuggest, allPackageWillOrder, activeRoute);
                        DirectionApiModel requestModel = DirectionApiModel.FromListGeoCoordinate(listPoints);
                        List<ResponsePolyLineModel> listPolyline = await _mapboxService.GetPolyLine(requestModel);
                        if (listPolyline.Count > 0)
                        {
                            if (directionSuggest == DirectionTypeConstant.FORWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + activeRoute.DistanceForward;
                                if (!isMaxSpacingError)
                                {
                                    await SendNotificationNewPackage(account.Id);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.BACKWARD)
                            {
                                bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + activeRoute.DistanceBackward;
                                if (!isMaxSpacingError)
                                {
                                    await SendNotificationNewPackage(account.Id);
                                }
                            }
                            else if (directionSuggest == DirectionTypeConstant.TWO_WAY)
                            {
                                {
                                    bool isMaxSpacingError = listPolyline[0].Distance > spacingValid + activeRoute.DistanceForward + activeRoute.DistanceBackward;
                                    if (!isMaxSpacingError)
                                    {
                                        await SendNotificationNewPackage(account.Id);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        private async Task SendNotificationNewPackage(Guid accountId)
        {
            SendNotificationModel model = new SendNotificationModel()
            {
                AccountId = accountId,
                Title = "Có gói hàng phù hợp",
                Body = "Có gói hàng phù hợp với lộ trình của bạn, nhanh tay vào và nhận thôi!!!"
            };
            try
            {
                await _fcmService.SendNotification(model);
            }
            catch (Exception ex)
            {
                _logger.LogError("FCM exception" + ex.Message);
            }
        }

        public async Task<List<RoutePoint>> GetRouteVirtual(List<GeoCoordinate> orderPoints, Guid routeId)
        {
            List<ResponsePolyLineModel> polylines = await _mapboxService.GetPolyLine(DirectionApiModel.FromListGeoCoordinate(orderPoints));
            List<RoutePoint> routePoints = new List<RoutePoint>();
            List<CoordinateApp>? points = polylines[0].PolyPoints;
            if (polylines[0].PolyPoints == null) new ArgumentNullException("polyline is null");
            int count = points!.Count;
            for (int i = 0; i < count; i++)
            {
                CoordinateApp point = points[i];
                RoutePoint routePoint = new RoutePoint()
                {
                    RouteId = routeId,
                    Index = i,
                    Latitude = point.Latitude,
                    Longitude = point.Longitude,
                    DirectionType = "VIRTUAL",
                    IsVitual = true
                };
                routePoints.Add(routePoint);
            }


            return routePoints;
        }

        public async Task<List<RoutePoint>> GetRouteVirtualGoong(List<GeoCoordinate> orderPoints, Guid routeId)
        {
            List<ResponsePolyLineModel> polylines = await _goongService.GetPolyLine(DirectionApiModel.FromListGeoCoordinate(orderPoints));
            List<RoutePoint> routePoints = new List<RoutePoint>();
            List<CoordinateApp>? points = polylines[0].PolyPoints;
            if (polylines[0].PolyPoints == null) new ArgumentNullException("polyline is null");
            int count = points!.Count;
            for (int i = 0; i < count; i++)
            {
                CoordinateApp point = points[i];
                RoutePoint routePoint = new RoutePoint()
                {
                    RouteId = routeId,
                    Index = i,
                    Latitude = point.Latitude,
                    Longitude = point.Longitude,
                    DirectionType = "VIRTUAL",
                    IsVitual = true
                };
                routePoints.Add(routePoint);
            }


            return routePoints;
        }

        public async Task ReloadVirtualRoute(Guid deliverId)
        {
            RouteEntity activeRoute = await _accountUtils.GetActiveRoute(deliverId);
            await RemoveRouteVirtual(activeRoute.Id);
            await CreateRouteVirtual(deliverId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveRouteVirtual(Guid routeId)
        {

            List<RoutePoint> routePoints = await _routePointRepo.GetAllAsync(predicate: routePoint => routePoint.RouteId == routeId && routePoint.IsVitual == true, disableTracking: true);
            _routePointRepo.DeleteRange(routePoints);

        }

        public async Task CreateRouteVirtual(Guid deliverId)
        {
            List<string> statusNotComplete = new List<string>
            {
                PackageStatus.SELECTED, PackageStatus.PICKUP_SUCCESS
            };
            List<Package> packagesNotComplete = await _packageRepo.GetAllAsync(predicate: package => package.DeliverId == deliverId && statusNotComplete.Contains(package.Status));

            List<GeoCoordinate> orderPoints = new List<GeoCoordinate>();

            Account? deliver = await _accountRepo.FirstOrDefaultAsync(predicate: account => account.Id == deliverId, include: source => source.Include(ac => ac.InfoUser));
            string directionSuggest = _configUserRepo.GetDirectionSuggest(deliver!.InfoUser!.Id);
            RouteEntity activeRoute = await _accountUtils.GetActiveRoute(deliverId);
            List<GeoCoordinate> geoCoordinates = SuggestPackageHelper.GetListPointOrder(directionSuggest, packagesNotComplete, activeRoute);
            if (geoCoordinates.Count <= 2) return;
            /*  if (packagesNotComplete.Count == 1)
              {
                  geoCoordinates = MapHelper.GetListPointOrder(directionSuggest, packagesNotComplete[0], activeRoute);
              }
              else if (packagesNotComplete.Count == 2)
              {
                  geoCoordinates = MapHelper.GetListPointOrderSecond(directionSuggest, packagesNotComplete[0], packagesNotComplete[1], activeRoute);
              }
              else if (packagesNotComplete.Count == 0) {
                  return;
              }*/
            // List<RoutePoint> virtualRoute = await GetRouteVirtual(geoCoordinates, activeRoute.Id);
            List<RoutePoint> virtualRoute = await GetRouteVirtual(geoCoordinates, activeRoute.Id);
            await _routePointRepo.InsertAsync(virtualRoute);
        }

        

    }
}
