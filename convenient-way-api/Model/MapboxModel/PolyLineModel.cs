using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;

namespace ship_convenient.Model.MapboxModel
{
    public class PolyLineModel
    {
        public double? Distance { get; set; }
        public double? Time { get; set; }
        public string? FromName { get; set; } = string.Empty;
        public string? ToName { get; set; } = string.Empty;
        public GeoCoordinate? From { get; set; }
        public GeoCoordinate? To { get; set; }
        public List<GeoCoordinate>? PolyPoints { get; set; }

        public PolyLineModel() { }

        public PolyLineModel(JObject jsonMapbox)
        {
            if (jsonMapbox is not null)
            {
                this.Distance = double.Parse(jsonMapbox["routes"]![0]!["distance"]!.ToString());
                this.Time = double.Parse(jsonMapbox["routes"]![0]!["duration"]!.ToString());
                this.FromName = jsonMapbox["waypoints"]![0]!["name"]!.ToString();
                this.ToName = jsonMapbox["waypoints"]![1]!["name"]!.ToString();
                this.From = new GeoCoordinate(longitude: double.Parse(jsonMapbox["waypoints"]![0]!["location"]![0]!.ToString()), latitude: double.Parse(jsonMapbox["waypoints"]![0]!["location"]![1]!.ToString()));
                this.To = new GeoCoordinate(longitude: double.Parse(jsonMapbox["waypoints"]![1]!["location"]![0]!.ToString()), latitude: double.Parse(jsonMapbox["waypoints"]![0]!["location"]![1]!.ToString()));
                this.PolyPoints = new List<GeoCoordinate>();
                int countPolyLine = jsonMapbox["routes"]![0]!["geometry"]!["coordinates"]!.Count();
                for (int i = 0; i < countPolyLine; i++)
                {
                    if (jsonMapbox["routes"]![0]!["geometry"]!["coordinates"]![i] is not null)
                    {
                        double longitudePoint = double.Parse(jsonMapbox["routes"]![0]!["geometry"]!["coordinates"]![i]![0]!.ToString());
                        double latitudePoint = double.Parse(jsonMapbox["routes"]![0]!["geometry"]!["coordinates"]![i]![1]!.ToString());
                        GeoCoordinate point = new GeoCoordinate(latitudePoint, longitudePoint);
                        this.PolyPoints.Add(point);
                    }
                }

            }
        }

        public static List<ResponsePolyLineModel> GetLines(JObject jsonMapbox)
        {
            List<ResponsePolyLineModel> listLine = new List<ResponsePolyLineModel>();

            if (jsonMapbox is not null)
            {
                int routeCount = jsonMapbox["routes"]!.Count();
                for (int i = 0; i < routeCount; i++)
                {
                    PolyLineModel lineMode = new PolyLineModel();
                    lineMode.Distance = double.Parse(jsonMapbox["routes"]![i]!["distance"]!.ToString());
                    lineMode.Time = double.Parse(jsonMapbox["routes"]![i]!["duration"]!.ToString());
                    lineMode.FromName = jsonMapbox["waypoints"]![i]!["name"]!.ToString();
                    lineMode.ToName = jsonMapbox["waypoints"]![1]!["name"]!.ToString();
                    lineMode.From = new GeoCoordinate(longitude: double.Parse(jsonMapbox["waypoints"]![i]!["location"]![i]!.ToString()), latitude: double.Parse(jsonMapbox["waypoints"]![i]!["location"]![1]!.ToString()));
                    lineMode.To = new GeoCoordinate(longitude: double.Parse(jsonMapbox["waypoints"]![1]!["location"]![i]!.ToString()), latitude: double.Parse(jsonMapbox["waypoints"]![i]!["location"]![1]!.ToString()));
                    lineMode.PolyPoints = new List<GeoCoordinate>();
                    int countPolyLine = jsonMapbox["routes"]![i]!["geometry"]!["coordinates"]!.Count();
                    for (int j = 0; j < countPolyLine; j++)
                    {
                        if (jsonMapbox["routes"]![i]!["geometry"]!["coordinates"]![j] is not null)
                        {
                            double longitudePoint = double.Parse(jsonMapbox["routes"]![i]!["geometry"]!["coordinates"]![j]![0]!.ToString());
                            double latitudePoint = double.Parse(jsonMapbox["routes"]![i]!["geometry"]!["coordinates"]![j]![1]!.ToString());
                            GeoCoordinate point = new GeoCoordinate(latitudePoint, longitudePoint);
                            lineMode.PolyPoints.Add(point);
                        }
                    }
                    listLine.Add(lineMode.ToResponseModel());
                }
            }
            return listLine;
        }

        public static List<ResponsePolyLineModel> GetLinesGoong(JObject jsonMapbox)
        {
            List<ResponsePolyLineModel> listLine = new List<ResponsePolyLineModel>();

            if (jsonMapbox is not null)
            {
                int routeCount = jsonMapbox["routes"]!.Count();
                for (int i = 0; i < routeCount; i++)
                {
                    PolyLineModel lineMode = new PolyLineModel();
                    lineMode.Distance = double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["distance"]!["value"]!.ToString());
                    lineMode.Time = double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["duration"]!["value"]!.ToString());
                    lineMode.FromName = jsonMapbox["routes"]![i]!["legs"]![0]!["start_address"]!.ToString();
                    lineMode.ToName = jsonMapbox["routes"]![i]!["legs"]![0]!["end_address"]!.ToString();
                    lineMode.From = new GeoCoordinate(longitude: double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["start_location"]!["lng"]!.ToString()), latitude: double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["start_location"]!["lat"]!.ToString()));
                    lineMode.To = new GeoCoordinate(longitude: double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["end_location"]!["lng"]!.ToString()), latitude: double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["end_location"]!["lat"]!.ToString()));
                    lineMode.PolyPoints = new List<GeoCoordinate>();
                    int countPolyLine = jsonMapbox["routes"]![i]!["legs"]![0]!["steps"]!.Count();
                    for (int j = 0; j < countPolyLine; j++)
                    {
                        if (jsonMapbox["routes"]![i]!["legs"]![0]!["steps"]![j] is not null)
                        {
                            double longitudePoint = double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["steps"]![j]!["start_location"]!["lng"]!.ToString());
                            double latitudePoint = double.Parse(jsonMapbox["routes"]![i]!["legs"]![0]!["steps"]![j]!["start_location"]!["lat"]!.ToString());
                            GeoCoordinate point = new GeoCoordinate(latitudePoint, longitudePoint);
                            lineMode.PolyPoints.Add(point);
                        }
                    }
                    listLine.Add(lineMode.ToResponseModel());
                }
            }
            return listLine;
        }

        public ResponsePolyLineModel ToResponseModel()
        {
            ResponsePolyLineModel model = new ResponsePolyLineModel();
            model.Distance = this.Distance;
            model.Time = this.Time;
            model.FromName = this.FromName;
            model.ToName = this.ToName;
            if (this.From is not null) model.From = this.From.ToCoordinate();
            if (this.To is not null) model.To = this.To.ToCoordinate();
            if (this.PolyPoints is not null)
            {
                model.PolyPoints = new List<CoordinateApp>();
                int polyLineCount = this.PolyPoints.Count;
                for (int i = 0; i < polyLineCount; i++)
                {
                    model.PolyPoints.Add(this.PolyPoints[i].ToCoordinate());
                }
            }
            return model;
        }

    }
}
