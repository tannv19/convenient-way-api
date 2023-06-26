using GeoCoordinatePortable;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Entities;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Model.RouteModel;
using ship_convenient.Services.PackageService;
using RouteEntity = ship_convenient.Entities.Route;

namespace ship_convenient.Helper
{
    public class MapHelper
    {
        public static bool ValidDestinationBetweenShipperAndPackage(PolyLineModel polyLine, Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<GeoCoordinate>? geoCoordinateList = polyLine.PolyPoints;
            if (geoCoordinateList != null)
            {
                int validNumberCoord = 0;
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate shopCoordinate = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceShop = geoCoordinate.GetDistanceTo(shopCoordinate);
                    if (distanceShop <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate packageCoordinate = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distancePackage = geoCoordinate.GetDistanceTo(packageCoordinate);
                    if (distancePackage <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                if (validNumberCoord == 2)
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool ValidDestinationBetweenDeliverAndPackage(List<RoutePoint> points, Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<GeoCoordinate>? geoCoordinateList = points.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            if (geoCoordinateList != null)
            {
                int validNumberCoord = 0;
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate shopCoordinate = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceShop = geoCoordinate.GetDistanceTo(shopCoordinate);
                    if (distanceShop <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate packageCoordinate = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distancePackage = geoCoordinate.GetDistanceTo(packageCoordinate);
                    if (distancePackage <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                if (validNumberCoord == 2)
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool ValidSuggestDirectionPackage(string direction, Package package, RouteEntity route)
        {
            bool result = false;
            GeoCoordinate startPackage = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
            GeoCoordinate destinationPackage = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
            if (direction == DirectionTypeConstant.FORWARD)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartPackage = startRoute.GetDistanceTo(startPackage);
                double startToEndPackage = startRoute.GetDistanceTo(destinationPackage);
                if (startToStartPackage < startToEndPackage)
                {
                    result = true;
                }
            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double endToStartPackage = endRoute.GetDistanceTo(startPackage);
                double endToEndPackage = endRoute.GetDistanceTo(destinationPackage);
                if (endToStartPackage < endToEndPackage)
                {
                    result = true;
                }
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                result = true;

            }
            return result;
        }

        public static List<GeoCoordinate> GetListPointOrder(string direction, Package package, RouteEntity route) {
            List<GeoCoordinate> orderPoints = new List<GeoCoordinate>();
            GeoCoordinate startPackage = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
            GeoCoordinate destinationPackage = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
            if (direction == DirectionTypeConstant.FORWARD)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartPackage = startRoute.GetDistanceTo(startPackage);
                double startToEndPackage = startRoute.GetDistanceTo(destinationPackage);
                if (startToStartPackage < startToEndPackage)
                {
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                }
            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double endToStartPackage = endRoute.GetDistanceTo(startPackage);
                double endToEndPackage = endRoute.GetDistanceTo(destinationPackage);
                if (endToStartPackage < endToEndPackage)
                {
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                }
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                orderPoints.Add(startPackage);
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                orderPoints.Add(destinationPackage);
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
            }
            return orderPoints;
        }

        public static List<GeoCoordinate> GetListPointOrderSecond(string direction, Package oldPackage,Package newPackage, RouteEntity route)
        {
            List<GeoCoordinate> orderPoints = new List<GeoCoordinate>();
            GeoCoordinate startOldPackage = new GeoCoordinate(oldPackage.StartLatitude, oldPackage.StartLongitude);
            GeoCoordinate destinationOldPackage = new GeoCoordinate(oldPackage.DestinationLatitude, oldPackage.DestinationLongitude);
            GeoCoordinate startNewPackage = new GeoCoordinate(newPackage.StartLatitude, newPackage.StartLongitude);
            GeoCoordinate destinationNewPackage = new GeoCoordinate(newPackage.DestinationLatitude, newPackage.DestinationLongitude);
            if (direction == DirectionTypeConstant.FORWARD)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartOldPackage = startRoute.GetDistanceTo(startOldPackage);
                double startToEndOldPackage = startRoute.GetDistanceTo(destinationOldPackage);
                double startToStartNewPackage = startRoute.GetDistanceTo(startNewPackage);
                double startToEndNewPackage = startRoute.GetDistanceTo(destinationNewPackage);
                List<double> distances = new List<double> {
                    startToStartOldPackage,
                    startToEndOldPackage,
                    startToStartNewPackage,
                    startToEndNewPackage
                };
                distances.Sort();
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                if (distances[0] == startToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[0] == startToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[0] == startToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[0] == startToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[1] == startToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[1] == startToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[1] == startToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[1] == startToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[2] == startToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[2] == startToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[2] == startToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[2] == startToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[3] == startToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[3] == startToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[3] == startToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[3] == startToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
              
            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double endToStartOldPackage = endRoute.GetDistanceTo(startOldPackage);
                double endToEndOldPackage = endRoute.GetDistanceTo(destinationOldPackage);
                double endToStartNewPackage = endRoute.GetDistanceTo(startNewPackage);
                double endToEndNewPackage = endRoute.GetDistanceTo(destinationNewPackage);
                List<double> distances = new List<double> {
                    endToStartOldPackage,
                    endToEndOldPackage,
                    endToStartNewPackage,
                    endToEndNewPackage
                };
                distances.Sort();
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));

                if (distances[0] == endToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[0] == endToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[0] == endToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[0] == endToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[1] == endToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[1] == endToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[1] == endToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[1] == endToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[2] == endToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[2] == endToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[2] == endToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[2] == endToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                if (distances[3] == endToStartOldPackage)
                {
                    orderPoints.Add(startOldPackage);
                }
                else if (distances[3] == endToEndOldPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                }
                else if (distances[3] == endToStartNewPackage)
                {
                    orderPoints.Add(startNewPackage);
                }
                else if (distances[3] == endToEndNewPackage)
                {
                    orderPoints.Add(destinationNewPackage);
                }

                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double startToStartOldPackage = startRoute.GetDistanceTo(startOldPackage);
                double startToStartNewPackage = startRoute.GetDistanceTo(startNewPackage);
                double endToEndOldPackage = endRoute.GetDistanceTo(destinationOldPackage);
                double endToEndNewPackage = endRoute.GetDistanceTo(destinationNewPackage);
                
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                if (startToStartOldPackage < startToStartNewPackage)
                {
                    orderPoints.Add(startOldPackage);
                    orderPoints.Add(startNewPackage);
                }
                else
                {
                    orderPoints.Add(startNewPackage);
                    orderPoints.Add(startOldPackage);
                }
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                if (endToEndOldPackage < endToEndNewPackage)
                {
                    orderPoints.Add(destinationOldPackage);
                    orderPoints.Add(destinationNewPackage);
                }
                else
                {
                    orderPoints.Add(destinationNewPackage);
                    orderPoints.Add(destinationOldPackage);
                }
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
            }
            return orderPoints;
        }



        public static bool IsValidPackageWithRouteTwoWay(List<RoutePoint> points,
            Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<RoutePoint> forwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            List<RoutePoint> backwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateForward = forwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            List<GeoCoordinate> geoCoordinateBackward = backwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result = true;
            }

            return result;
        }

        public static PackageDistanceValid IsValidPackageWithRouteTwoWayV2(List<RoutePoint> points,
            Package package, double spacingValid = 2000)
        {
            PackageDistanceValid result = new();
            List<RoutePoint> forwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            List<RoutePoint> backwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateForward = forwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            List<GeoCoordinate> geoCoordinateBackward = backwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    result.StartPointMinDistance = distanceShop;
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    result.EndPointMinDistance = distancePackage;
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result.IsValid = true;
            }
            else {
                result.IsValid = false;
                List<double> distanceWithStartPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
                {
                    GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageStart);
                    distanceWithStartPoint.Add(distanceStart);
                }
                double minDistanceStart = distanceWithStartPoint.Min();

                List<double> distanceWithEndPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
                {
                    GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageEnd);
                    distanceWithEndPoint.Add(distanceStart);
                }
                double minDistanceEnd = distanceWithEndPoint.Min();

                result.StartPointMinDistance = minDistanceStart;
                result.EndPointMinDistance = minDistanceEnd;
            }

            return result;
        }


        public static bool IsValidPackageWithRouteForward(List<RoutePoint> points,
            Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<RoutePoint> forwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateForward = forwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result = true;
            }

            return result;
        }

        public static PackageDistanceValid IsValidPackageWithRouteForwardV2(List<RoutePoint> points,
            Package package, double spacingValid = 2000)
        {
            PackageDistanceValid result = new();
            List<RoutePoint> forwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.FORWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateForward = forwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    result.StartPointMinDistance = distanceShop;
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    result.EndPointMinDistance = distancePackage;
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result.IsValid = true;
            }
            else {
                result.IsValid = false;
                List<double> distanceWithStartPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
                {
                    GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageStart);
                    distanceWithStartPoint.Add(distanceStart);
                }
                double minDistanceStart = distanceWithStartPoint.Min();

                List<double> distanceWithEndPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateForward)
                {
                    GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageEnd);
                    distanceWithEndPoint.Add(distanceStart);
                }
                double minDistanceEnd = distanceWithEndPoint.Min();
                result.StartPointMinDistance = minDistanceStart;
                result.EndPointMinDistance = minDistanceEnd;
            }

            return result;
        }

        public static bool IsValidPackageWithRouteBackward(List<RoutePoint> points,
           Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<RoutePoint> backwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateBackward = backwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result = true;
            }

            return result;
        }

        public static PackageDistanceValid IsValidPackageWithRouteBackwardV2(List<RoutePoint> points,
           Package package, double spacingValid = 2000)
        {
            PackageDistanceValid result = new();
            List<RoutePoint> backwardRoute = points.Where(routePoint => routePoint.DirectionType == DirectionTypeConstant.BACKWARD)
                        .OrderBy(source => source.Index).ToList();
            List<GeoCoordinate> geoCoordinateBackward = backwardRoute.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
            int validNumberCoord = 0;
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                double distanceShop = geoCoordinate.GetDistanceTo(packageStart);
                if (distanceShop <= spacingValid)
                {
                    result.StartPointMinDistance = distanceShop;
                    validNumberCoord++;
                    break;
                };
            }
            foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
            {
                GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                double distancePackage = geoCoordinate.GetDistanceTo(packageEnd);
                if (distancePackage <= spacingValid)
                {
                    result.EndPointMinDistance = distancePackage;
                    validNumberCoord++;
                    break;
                };
            }
            if (validNumberCoord == 2)
            {
                result.IsValid = true;
            }
            else {
                result.IsValid = false;
                List<double> distanceWithStartPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
                {
                    GeoCoordinate packageStart = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageStart);
                    distanceWithStartPoint.Add(distanceStart);
                }
                double minDistanceStart = distanceWithStartPoint.Min();

                List<double> distanceWithEndPoint = new();
                foreach (GeoCoordinate geoCoordinate in geoCoordinateBackward)
                {
                    GeoCoordinate packageEnd = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distanceStart = geoCoordinate.GetDistanceTo(packageEnd);
                    distanceWithEndPoint.Add(distanceStart);
                }
                double minDistanceEnd = distanceWithEndPoint.Min();
                result.StartPointMinDistance = minDistanceStart;
                result.EndPointMinDistance = minDistanceEnd;
            }

            return result;
        }

        public static bool IsTrueWithPackageAndUserRoute(string directionType, List<RoutePoint> routePoints,
            RouteEntity route, Package package, double spacingValid = 2000)
        {
            bool result = false;
            if (directionType == DirectionTypeConstant.TWO_WAY)
            {
                result = IsValidPackageWithRouteTwoWay(routePoints, package, spacingValid);
            }
            else if (directionType == DirectionTypeConstant.FORWARD)
            {
                bool isValidSuggestDirection = ValidSuggestDirectionPackage(directionType, package, route);
                result = IsValidPackageWithRouteForward(routePoints, package, spacingValid) && isValidSuggestDirection;
            }
            else if (directionType == DirectionTypeConstant.BACKWARD)
            {
                bool isValidSuggestDirection = ValidSuggestDirectionPackage(directionType, package, route);
                result = IsValidPackageWithRouteBackward(routePoints, package, spacingValid) && isValidSuggestDirection;
            }
            
            return result;
        }

        public static PackageDistanceValid IsTrueWithPackageAndUserRouteV2(string directionType, List<RoutePoint> routePoints,
            RouteEntity route, Package package, double spacingValid = 2000)
        {
            PackageDistanceValid result = new();
            if (directionType == DirectionTypeConstant.TWO_WAY)
            {
                result = IsValidPackageWithRouteTwoWayV2(routePoints, package, spacingValid);
               
            }
            else if (directionType == DirectionTypeConstant.FORWARD)
            {
                bool isValidSuggestDirection = ValidSuggestDirectionPackage(directionType, package, route);
                PackageDistanceValid packageDistance = IsValidPackageWithRouteForwardV2(routePoints, package, spacingValid);
                result.StartPointMinDistance = packageDistance.StartPointMinDistance;
                result.EndPointMinDistance = packageDistance.EndPointMinDistance;
                if (isValidSuggestDirection && packageDistance.IsValid)
                {
                    result.IsValid = true;
             
                }
                else
                {
                    result.IsValid = false;
       
                }

            }
            else if (directionType == DirectionTypeConstant.BACKWARD)
            {
                bool isValidSuggestDirection = ValidSuggestDirectionPackage(directionType, package, route);
                PackageDistanceValid packageDistance = IsValidPackageWithRouteBackwardV2(routePoints, package, spacingValid);
                result.StartPointMinDistance = packageDistance.StartPointMinDistance;
                result.EndPointMinDistance = packageDistance.EndPointMinDistance;
                if (isValidSuggestDirection && packageDistance.IsValid)
                {
                    result.IsValid = true;
                }
                else
                {
                    result.IsValid = false;
                
                }
            }

            return result;
        }

        
    }
        
    }

