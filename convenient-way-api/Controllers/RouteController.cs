using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Helper.SuggestPackageHelper;
using ship_convenient.Model.RouteModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.RouteService;
using Swashbuckle.AspNetCore.Annotations;

namespace ship_convenient.Controllers
{
    public class RouteController : BaseApiController
    {
        private readonly IRouteService _routeService;
        private readonly IAccountService _accountService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService,IAccountService accountService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("{accountId}")]
        [SwaggerOperation(Summary = "Get route with accountId")]
        [ProducesResponseType(typeof(ApiResponse<ResponseRouteModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult>
            GetAccountRoute(Guid accountId)
        {
            try
            {
                ApiResponse<List<ResponseRouteModel>> response =
                    await _routeService.GetRouteUserId(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get shipper route : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost()]
        [SwaggerOperation(Summary = "Create route")]
        [ProducesResponseType(typeof(ApiResponse<ResponseRouteModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRoute(CreateRouteModel model)
        {
            try
            {
                ApiResponse<ResponseRouteModel> response = await _routeService.Create(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Create route exception : " + ex.Message.Substring(0, 100));
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("active-route")]
        [SwaggerOperation(Summary = "Set active route")]
        [ProducesResponseType(typeof(ActionResult<ApiResponse<List<ResponseRouteModel>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetActiveRoute(Guid id)
        {
            try
            {
                ApiResponse response = await _routeService.SetActiveRoute(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Set active route : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            try
            {
                ApiResponse response = await _routeService.Delete(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete route : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("point-list/{routeId}")]
        [ProducesResponseType(typeof(ApiResponse<ResponseRoutePointListModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListRoutePoint(Guid routeId)
        {

            try
            {
                ApiResponse<ResponseRoutePointListModel> response =
                    await _routeService.GetPointList(routeId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list route point : " + ex.Message);
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("point-virtual/{accountId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseRoutePointModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListRoutePointVirtual(Guid accountId)
        {

            try
            {
                ApiResponse<List<ResponseRoutePointModel>> response =
                    await _routeService.GetPointListVirtual(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list route point : " + ex.Message);
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("order-point-virtual/{accountId}")]
        [ProducesResponseType(typeof(ApiResponse<List<DistancePackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListOrderPointVirtual(Guid accountId)
        {

            try
            {
                ApiResponse<List<DistancePackageModel>> response =
                    await _accountService.GetOrderPointVirtual(accountId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get order point virtual : " + ex.Message);
                return StatusCode(500, ex.Message);
            }

        }
    }
}
