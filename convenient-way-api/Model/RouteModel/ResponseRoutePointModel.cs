namespace ship_convenient.Model.RouteModel
{
    public class ResponseRoutePointModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Index { get; set; }
        public string DirectionType { get; set; } = string.Empty;
        public Guid RouteId { get; set; }
        public bool IsVitual { get; set; } = false;
    }
}
