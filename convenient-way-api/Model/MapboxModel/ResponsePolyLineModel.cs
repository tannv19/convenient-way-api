namespace ship_convenient.Model.MapboxModel
{
    public class ResponsePolyLineModel
    {
        public double? Distance { get; set; }
        public double? Time { get; set; }
        public string? FromName { get; set; } = string.Empty;
        public string? ToName { get; set; } = string.Empty;
        public CoordinateApp? From { get; set; }
        public CoordinateApp? To { get; set; }
        public List<CoordinateApp>? PolyPoints { get; set; }
    }
}
