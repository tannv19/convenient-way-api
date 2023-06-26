using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Model.RouteModel
{
    public class CreateRouteModel
    {
        public string FromName { get; set; } = string.Empty;
        public double FromLongitude { get; set; }
        public double FromLatitude { get; set; }
        public string ToName { get; set; } = string.Empty;
        public double ToLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double DistanceForward { get; set; }
        public double DistanceBackward { get; set; }
        public Guid AccountId { get; set; }
        public List<CreateRoutePointModel> RoutePoints { get; set; } = new();

        public Route ConvertToEntity(Guid infoUserId)
        {
            Route route = new Route();
            route.FromName = this.FromName;
            route.FromLatitude = this.FromLatitude;
            route.FromLongitude = this.FromLongitude;
            route.ToName = this.ToName;
            route.ToLatitude = this.ToLatitude;
            route.ToLongitude = this.ToLongitude;
            route.DistanceForward = this.DistanceForward;
            route.DistanceBackward = this.DistanceBackward;
            route.InfoUserId = infoUserId;
            route.RoutePoints = this.RoutePoints.Select(x => x.ToEntity()).ToList();
            return route;
        }
    }
}
