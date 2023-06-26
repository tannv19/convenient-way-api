using Newtonsoft.Json.Linq;

namespace ship_convenient.Model.GoongModel
{
    public class ResponseSearchModel
    {
        public string Name { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public ResponseSearchModel()
        {

        }

        public ResponseSearchModel(JObject jObject)
        {
            this.Name = jObject["formatted_address"]!.ToString();
            this.PlaceId = jObject["place_id"]!.ToString();
            this.Longitude = double.Parse(jObject["geometry"]!["location"]!["lng"]!.ToString());
            this.Latitude = double.Parse(jObject["geometry"]!["location"]!["lat"]!.ToString());
        }

        public ResponseSearchModel(JToken jObject)
        {
            this.Name = jObject["formatted_address"]!.ToString();
            this.PlaceId = jObject["place_id"]!.ToString();
            this.Longitude = double.Parse(jObject["geometry"]!["location"]!["lng"]!.ToString());
            this.Latitude = double.Parse(jObject["geometry"]!["location"]!["lat"]!.ToString());
        }
    }
}
