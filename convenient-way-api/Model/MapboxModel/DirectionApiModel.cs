using GeoCoordinatePortable;

namespace ship_convenient.Model.MapboxModel
{
    public class DirectionApiModel
    {
        public CoordinateApp From { get; set; } = new CoordinateApp();
        public List<CoordinateApp> To { get; set; } = new List<CoordinateApp>();


        public string GetCoordsQuery()
        {
            string result = "";
            result += From.Longitude + "," + From.Latitude + ";";
            foreach (var item in To)
            {
                result += item.Longitude + "," + item.Latitude + ";";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }

        public string GetCoordsQueryGoong()
        {
            string result = "&origin=";
            result += From.Latitude + "," + From.Longitude + "&destination=";
            foreach (var item in To)
            {
                result += item.Latitude + "," + item.Longitude + ";";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }

        static public DirectionApiModel FromListGeoCoordinate(List<GeoCoordinate> data) {
            DirectionApiModel result = new DirectionApiModel();
            result.From = new CoordinateApp(data[0].Longitude, data[0].Latitude);
            for (int i = 1; i < data.Count; i++)
            {
                result.To.Add(new CoordinateApp(data[i].Longitude, data[i].Latitude));
            }
            return result;
        }
    }
}
