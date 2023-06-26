using Newtonsoft.Json.Linq;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.GoongModel;
using ship_convenient.Model.MapboxModel;

namespace ship_convenient.Services.GoongService
{
    public interface IGoongService
    {
        Task<JObject> SearchApi(string search, double longitude, double latitude);
        Task<ApiResponse<ResponseSearchModel?>> DetailPlaceApi(string placeId);
        Task<ApiResponse<List<ResponseSearchModel>>> GeocodingLocation(double longitude, double latitude);
        Task<ApiResponse<List<ResponseSearchModel>>> SearchLocation(string search, double longitude, double latitude);
        Task<ApiResponse<List<ResponseSearchDefaultModel>>> SearchLocationDefault(string search, double longitude, double latitude);
        Task<List<ResponsePolyLineModel>> GetPolyLine(DirectionApiModel model);
    }
}
