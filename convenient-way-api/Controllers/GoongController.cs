using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.GoongModel;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Services.GoongService;
using Swashbuckle.AspNetCore.Annotations;

namespace ship_convenient.Controllers
{
    public class GoongController : BaseApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IGoongService _goongService;
        private readonly ILogger<GoongController> _logger;

        public GoongController(IConfiguration configuration, IGoongService goongService, ILogger<GoongController> logger)
        {
            _configuration = configuration;
            _goongService = goongService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(summary: "Default search only location in HCM city")]
        public async Task<ActionResult<ApiResponse<List<ResponseSearchModel>>>> SearchApi(string search, double longitude = 106.64849987615482, double latitude = 10.807953964793464)
        {
            try
            {
                ApiResponse<List<ResponseSearchModel>> response = await _goongService.SearchLocation(search, longitude, latitude);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api goongng : " + ex.Message.Substring(0, 200));
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search-default")]
        [SwaggerOperation(summary: "Default search only location in HCM city")]
        public async Task<ActionResult<ApiResponse<List<ResponseSearchDefaultModel>>>> SearchApiDefault(string search, double longitude = 106.64849987615482, double latitude = 10.807953964793464)
        {
            try
            {
                ApiResponse<List<ResponseSearchDefaultModel>> response = await _goongService.SearchLocationDefault(search, longitude, latitude);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api goongng : " + ex.Message.Substring(0, 200));
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("detail")]
        [SwaggerOperation(summary: "Default search only location in HCM city")]
        public async Task<ActionResult<ApiResponse<ResponseSearchModel>>> GetDetailPlaceId(string placeId)
        {
            try
            {
                ApiResponse<ResponseSearchModel?> response = await _goongService.DetailPlaceApi(placeId);
                if (response.Success == false)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api goongng : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("geocoding")]
        [SwaggerOperation(summary: "Geocoding coordinate")]
        public async Task<ActionResult<ApiResponse<List<ResponseSearchModel>>>> GeocodingApi(
            double longitude = 106.8104692523854, double latitude = 10.840967162054827)
        {
            try
            {
                ApiResponse<List<ResponseSearchModel>> response = await _goongService.GeocodingLocation(longitude, latitude);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api goongng : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("direction")]
        [SwaggerOperation(summary: "Direction")]
        public async Task<ActionResult<ApiResponse<List<ResponsePolyLineModel>>>> DirectionApi(
            DirectionApiModel model)
        {
            try
            {
                List<ResponsePolyLineModel> polyLineModel = await _goongService.GetPolyLine(model);
                return Ok(new ApiResponse<List<ResponsePolyLineModel>>
                {
                    Success = polyLineModel == null ? false : true,
                    Message = polyLineModel == null ? "Thông tin tọa độ bị sai" : "Lấy thông tin thành công từ Goong",
                    Data = polyLineModel,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception api goongng : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
