using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;
using ship_convenient.Model.MapboxModel;

namespace ship_convenient.Services.MapboxService
{
    public interface IMapboxService
    {
        Task<JObject> DirectionApi(DirectionApiModel model);
        Task<JObject> SearchApi(string search);
        Task<List<ResponsePolyLineModel>> GetPolyLine(DirectionApiModel model);
        Task<PolyLineModel> GetPolyLineModel(GeoCoordinate From, GeoCoordinate To);
    }
}
