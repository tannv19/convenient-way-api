using GeoCoordinatePortable;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Entities;
using RouteEntity = ship_convenient.Entities.Route;

namespace ship_convenient.Helper.SuggestPackageHelper
{
    public class SuggestPackageHelper
    {
        public static List<GeoCoordinate> GetListPointOrder(string direction, List<Package> packages, RouteEntity route)
        {
            List<GeoCoordinate> orderPoints = new List<GeoCoordinate>();
            
            if (direction == DirectionTypeConstant.FORWARD)
            {
                List<DistancePackageModel> distancesPoint = new List<DistancePackageModel>();
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(startRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start,package.StartLatitude,package.StartLongitude, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(startRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination,package.DestinationLatitude, package.DestinationLongitude, package);
                    distancesPoint.Add(startPoint);
                    distancesPoint.Add(destinationPoint);
                });
                distancesPoint = distancesPoint.OrderBy(source => source.Distance).ToList();
                
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                distancesPoint.ForEach(point => {
                    orderPoints.Add(new GeoCoordinate(point.Latitude, point.Longitude));
                });
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));

            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                List<DistancePackageModel> distancesPoint = new List<DistancePackageModel>();
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start, package.StartLatitude, package.StartLongitude, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination, package.DestinationLatitude, package.DestinationLongitude, package);
                    distancesPoint.Add(startPoint);
                    distancesPoint.Add(destinationPoint);
                });

                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                distancesPoint.ForEach(point => {
                    orderPoints.Add(new GeoCoordinate(point.Latitude, point.Longitude));
                });
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                List<DistancePackageModel> distancesPointForward = new List<DistancePackageModel>();
                List<DistancePackageModel> distancesPointBackward = new List<DistancePackageModel>();
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);

                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start, package.StartLatitude, package.StartLongitude, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination, package.DestinationLatitude, package.DestinationLongitude, package);
                    distancesPointForward.Add(startPoint);
                    distancesPointBackward.Add(destinationPoint);
                });
                distancesPointForward = distancesPointForward.OrderBy(source => source.Distance).ToList();
                distancesPointBackward = distancesPointBackward.OrderBy(source => source.Distance).ToList();

                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                distancesPointForward.ForEach(point => {
                    orderPoints.Add(new GeoCoordinate(point.Latitude, point.Longitude));
                });
                orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                distancesPointBackward.ForEach(point => {
                    orderPoints.Add(new GeoCoordinate(point.Latitude, point.Longitude));
                });
                orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
            }
            return orderPoints;
        }


        public static List<DistancePackageModel> GetListPointOrderName(string direction, List<Package> packages, RouteEntity route)
        {
            List<DistancePackageModel> orderPoints = new List<DistancePackageModel>();
            DistancePackageModel startPointRoute = new DistancePackageModel(0, PointType.Deliver, route.FromLatitude, route.FromLongitude,route.FromName, null);
            DistancePackageModel destinationPointRoute = new DistancePackageModel(0, PointType.Deliver, route.ToLatitude, route.ToLongitude, route.ToName, null);   
   
            if (direction == DirectionTypeConstant.FORWARD)
            {
                List<DistancePackageModel> distancesPoint = new List<DistancePackageModel>();
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(startRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start, package.StartLatitude, package.StartLongitude,package.StartAddress, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(startRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination, package.DestinationLatitude, 
                        package.DestinationLongitude,package.DestinationAddress, package);
                    distancesPoint.Add(startPoint);
                    distancesPoint.Add(destinationPoint);
                });
                distancesPoint = distancesPoint.OrderBy(source => source.Distance).ToList();

                orderPoints.Add(startPointRoute);
                distancesPoint.ForEach(point => {
                    orderPoints.Add(point);
                });
                orderPoints.Add(destinationPointRoute);

            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                List<DistancePackageModel> distancesPoint = new List<DistancePackageModel>();
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start, package.StartLatitude, package.StartLongitude, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination, package.DestinationLatitude, package.DestinationLongitude, package);
                    distancesPoint.Add(startPoint);
                    distancesPoint.Add(destinationPoint);
                });

                orderPoints.Add(destinationPointRoute);
                distancesPoint.ForEach(point => {
                    orderPoints.Add(point);
                });
                orderPoints.Add(startPointRoute);
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                List<DistancePackageModel> distancesPointForward = new List<DistancePackageModel>();
                List<DistancePackageModel> distancesPointBackward = new List<DistancePackageModel>();
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);

                packages.ForEach(package => {
                    DistancePackageModel startPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.StartLatitude, package.StartLongitude)), PointType.Start, package.StartLatitude, package.StartLongitude, package);
                    DistancePackageModel destinationPoint = new DistancePackageModel(endRoute.GetDistanceTo(new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude)), PointType.Destination, package.DestinationLatitude, package.DestinationLongitude, package);
                    distancesPointForward.Add(startPoint);
                    distancesPointBackward.Add(destinationPoint);
                });
                distancesPointForward = distancesPointForward.OrderBy(source => source.Distance).ToList();
                distancesPointBackward = distancesPointBackward.OrderBy(source => source.Distance).ToList();

                orderPoints.Add(startPointRoute);
                distancesPointForward.ForEach(point => {
                    orderPoints.Add(point);
                });
                orderPoints.Add(destinationPointRoute);
                distancesPointBackward.ForEach(point => {
                    orderPoints.Add(point);
                });
                orderPoints.Add(startPointRoute);
            }
            return orderPoints;
        }
    }
}
