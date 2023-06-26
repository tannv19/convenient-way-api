using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;
using ship_convenient.Model.MapboxModel;

namespace ship_convenient.Services.MapboxService
{
    public class MapboxService : IMapboxService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MapboxService> _logger;
        public MapboxService(IConfiguration configuration, ILogger<MapboxService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<JObject> DirectionApi(DirectionApiModel model)
        {
            JObject result;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Mapbox:uriDirection"] + model.GetCoordsQuery() + _configuration["Mapbox:endUriDirection"])
            };
            _logger.LogDebug("Request mapbox uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                result = JObject.Parse(body);
            }
            return result;
        }

        public async Task<JObject> SearchApi(string search)
        {
            JObject result;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Mapbox:uriSearch"] + search + _configuration["Mapbox:endUriSearch"])
            };
            _logger.LogDebug("Request mapbox uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                result = JObject.Parse(body);
            }
            return result;
        }

        public async Task<List<ResponsePolyLineModel>> GetPolyLine(DirectionApiModel model)
        {
            List<ResponsePolyLineModel> result;
            JObject bodyResponse;
            HttpClient client = new HttpClient();
            
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Mapbox:uriDirection"] + model.GetCoordsQuery() + _configuration["Mapbox:endUriDirection"])
            };
            _logger.LogDebug("Request mapbox uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                bodyResponse = JObject.Parse(body);
                result = PolyLineModel.GetLines(bodyResponse);
                _logger.LogDebug("Time: " + result[0].Time + ", " + "Distance: " + result[0].Distance + " \n"
                    + "From name: " + result[0].FromName + ", " + "To name: " + result[0].ToName);

            }
            return result;
        }

        public async Task<PolyLineModel> GetPolyLineModel(GeoCoordinate From, GeoCoordinate To)
        {
            PolyLineModel result;
            JObject bodyResponse;
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Mapbox:uriDirection"] + From.Longitude + "," + From.Latitude + ";" + To.Longitude + "," + To.Latitude + _configuration["Mapbox:endUriDirection"])
            };
            _logger.LogDebug("Request mapbox uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                bodyResponse = JObject.Parse(body);
                result = new PolyLineModel(bodyResponse);
                _logger.LogInformation("Time: " + result.Time + ", " + "Distance: " + result.Distance + " \n"
                    + "From name: " + result.FromName + ", " + "To name: " + result.ToName);

            }
            return result;
        }


    }
}
