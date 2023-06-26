namespace ship_convenient.Model.RouteModel
{
    public class ResponseRoutePointListModel
    {
        public Guid RouteId { get; set; }
        public List<ResponseRoutePointModel> ForwardPoints { get; set; } = new();
        public List<ResponseRoutePointModel> BackwardPoints { get; set; } = new();
    }
}
