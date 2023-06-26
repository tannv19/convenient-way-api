using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Services.MapboxService;

namespace ship_convenient.Controllers
{
    public class MapboxController : BaseApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IMapboxService _mapboxService;
        private readonly ILogger<MapboxController> _logger;

        public MapboxController(IConfiguration configuration, IMapboxService mapboxService, ILogger<MapboxController> logger)
        {
            _configuration = configuration;
            _mapboxService = mapboxService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SearchApi(string search)
        {
            try
            {
                JObject directionApi = await _mapboxService.SearchApi(search);
                return Ok(new ApiResponse<JObject>
                {
                    Message = "Tìm kiếm",
                    Data = directionApi,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api mapbox : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("direction")]
        public async Task<ActionResult<ApiResponse<JObject>>> DirectionApi(DirectionApiModel model)
        {
            try
            {
                JObject directionApi = await _mapboxService.DirectionApi(model);
                return Ok(new ApiResponse<JObject>
                {
                    Success = directionApi == null ? false : true,
                    Message = directionApi == null ? "Thông tin tọa độ bị sai" : "Lấy thông tin thành công từ Mapbox",
                    Data = directionApi,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api mapbox : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("poly-line")]
        public async Task<ActionResult<ApiResponse<List<ResponsePolyLineModel>>>> PolyLineApi(DirectionApiModel model)
        {
            try
            {
                List<ResponsePolyLineModel> polyLineModel = await _mapboxService.GetPolyLine(model);
                return Ok(new ApiResponse<List<ResponsePolyLineModel>>
                {
                    Success = polyLineModel == null ? false : true,
                    Message = polyLineModel == null ? "Thông tin tọa độ bị sai" : "Lấy thông tin thành công từ Mapbox",
                    Data = polyLineModel,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api mapbox : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
