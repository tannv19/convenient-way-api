using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.DashboardModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.DashboardService;

namespace ship_convenient.Controllers
{

    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet("account-active")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseAccountModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccountsActive()
        {
            try
            {
                ApiResponse<List<ResponseAccountModel>> response = await _dashboardService.GetListAccountActive();
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("package-count")]
        [ProducesResponseType(typeof(ApiResponse<PackageCountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPackageCounts(Guid? deliverId, Guid? senderId)
        {
            try
            {
                ApiResponse<PackageCountModel> response = await _dashboardService.GetCountPackage(
                    deliverId: deliverId, senderId: senderId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        
    }
}
