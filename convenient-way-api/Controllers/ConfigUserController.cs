using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigUserModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.ConfigUserService;

namespace ship_convenient.Controllers
{
    [Route("api/v1.0/config-user")]
    public class ConfigUserController : BaseApiController
    {
        private readonly IConfigUserService _configUserService;

        public ConfigUserController(IConfigUserService configUserService)
        {
            _configUserService = configUserService;
        }

        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseConfigUserModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(Guid accountId)
        {
            var result = await _configUserService.GetList(accountId);
            return SendResponse(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ResponseConfigUserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateUserConfigModel model)
        {
            var result = await _configUserService.Update(model);
            return SendResponse(result);
        }

        [HttpPut("location")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationUserModel model)
        {
            var result = await _configUserService.UpdateLocationUser(model);
            return SendResponse(result);
        }

    }
}
