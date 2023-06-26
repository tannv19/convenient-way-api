using Newtonsoft.Json.Linq;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.GoongModel;
using ship_convenient.Model.MapboxModel;

namespace ship_convenient.Services.GoongService
{
    public class GoongService : IGoongService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoongService> _logger;

        public GoongService(IConfiguration configuration, ILogger<GoongService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<ResponseSearchModel?>> DetailPlaceApi(string placeId)
        {
            ApiResponse<ResponseSearchModel?> respone = new ApiResponse<ResponseSearchModel?>();
            ResponseSearchModel? model = await GetDetailPlaceId(placeId);
            if (model != null)
            {
                respone.ToSuccessResponse(model, "Lấy thông tin thành công");
            }
            else
            {
                respone.ToFailedResponse("Không tìm thấy kết quả");
            }
            return respone;
        }

        public async Task<ApiResponse<List<ResponseSearchModel>>> GeocodingLocation(double longitude, double latitude)
        {
            ApiResponse<List<ResponseSearchModel>> response = new ApiResponse<List<ResponseSearchModel>>();
            List<ResponseSearchModel> results = await GeocodingApi(longitude, latitude);
            if (results.Count > 0)
            {
                response.ToSuccessResponse(results, "Lấy thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Không tìm thấy dữ liệu");
            }
            return response;
        }

        private async Task<List<ResponseSearchModel>> GeocodingApi(double longitude, double latitude)
        {
            List<ResponseSearchModel> result = new List<ResponseSearchModel>();
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            string endUri = "&latlng=" + latitude + "," + longitude;
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriGeocoding"] + endUri)
            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    JObject jObject = JObject.Parse(body);
                    int count = jObject["results"]!.Count();
                    for (int i = 0; i < count; i++)
                    {
                        ResponseSearchModel item = new ResponseSearchModel(jObject["results"]![i]!);
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public async Task<JObject> SearchApi(string search, double longitude, double latitude)
        {
            JObject result;
            string endUri = $"&input={search}";
            if (longitude != 0 && latitude != 0)
            {
                endUri = endUri + $"&location={latitude},{longitude}";
            }

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriSearch"] + endUri)
            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                result = JObject.Parse(body);
            }
            return result;
        }

        public async Task<ApiResponse<List<ResponseSearchModel>>> SearchLocation(string search, double longitude, double latitude)
        {
            ApiResponse<List<ResponseSearchModel>> response = new ApiResponse<List<ResponseSearchModel>>();
            List<ResponseSearchModel> locations = new List<ResponseSearchModel>();
            List<string> placeIds = await GetListPlaceId(search, longitude, latitude);
            for (int i = 0; i < placeIds.Count; i++)
            {
                var detailPlace = await GetDetailPlaceId(placeIds[i]);
                if (detailPlace != null) locations.Add(detailPlace);
            }
            if (locations.Count > 0)
            {
                response.ToSuccessResponse(locations, "Lấy thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Không tìm thấy kết quả");
            }
            return response;
        }

        public async Task<ResponseSearchModel?> GetDetailPlaceId(string placeId)
        {
            ResponseSearchModel? result = null;
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriDetail"] + placeId)
            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    JObject jObject = JObject.Parse(body);
                    result = new ResponseSearchModel(jObject["result"]!);
                }
            }
            return result;
        }
        private async Task<List<string>> GetListPlaceId(string search, double longitude, double latitude)
        {
            List<string> result = new List<string>();
            string endUri = $"&input={search}";
            if (longitude != 0 && latitude != 0)
            {
                endUri = endUri + $"&location={latitude},{longitude}";
            }

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(200);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriSearch"] + endUri),
                
            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                if (!body.Equals(string.Empty))
                {
                    JObject jObject = JObject.Parse(body);
                    int count = jObject["predictions"]!.Count();
                    for (int i = 0; i < count; i++)
                    {
                        string placeId = jObject["predictions"]![i]!["place_id"]!.ToString();
                        result.Add(placeId);
                    }
                }
            }
            return result;
        }

        public async Task<ApiResponse<List<ResponseSearchDefaultModel>>> SearchLocationDefault(string search, double longitude, double latitude)
        {
            ApiResponse<List<ResponseSearchDefaultModel>> response = new();
            string endUri = $"&input={search}";
            if (longitude != 0 && latitude != 0)
            {
                endUri = endUri + $"&location={latitude},{longitude}";
            }

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriSearch"] + endUri),

            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            List<ResponseSearchDefaultModel> locations = new();
            using (var responseGoong = await client.SendAsync(request))
            {
                responseGoong.EnsureSuccessStatusCode();
                string body = await responseGoong.Content.ReadAsStringAsync();
                if (!body.Equals(string.Empty))
                {
                    JObject jObject = JObject.Parse(body);
                    int count = jObject["predictions"]!.Count();
                    for (int i = 0; i < count; i++)
                    {
                        string placeId = jObject["predictions"]![i]!["place_id"]!.ToString();
                        string description = jObject["predictions"]![i]!["description"]!.ToString();
                        locations.Add(new ResponseSearchDefaultModel(placeId, description));
                    }
                }
            }
            if (locations.Count > 0)
            {
                response.ToSuccessResponse(locations, "Lấy thông tin thành công");
            }
            else
            {
                response.ToSuccessResponse("Không tìm thấy kết quả");
            }
            return response;
        }

        public async Task<List<ResponsePolyLineModel>> GetPolyLine(DirectionApiModel model)
        {
            List<ResponsePolyLineModel> result;
            JObject bodyResponse;
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["Goong:uriDirection"] + model.GetCoordsQueryGoong())
            };
            _logger.LogInformation("Request goong uri: " + request.RequestUri);
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                bodyResponse = JObject.Parse(body);
                result = PolyLineModel.GetLinesGoong(bodyResponse);
                _logger.LogDebug("Time: " + result[0].Time + ", " + "Distance: " + result[0].Distance + " \n"
                    + "From name: " + result[0].FromName + ", " + "To name: " + result[0].ToName);

            }
            return result;
        }
    }
}
