using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigModel;
using ship_convenient.Services.ConfigService;

namespace ship_convenient.Controllers
{
    public class ConfigController : BaseApiController
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseConfigModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _configService.GetAll();
            return SendResponse(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ConfigApp>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateConfigModel model)
        {
            var result = await _configService.Update(model);
            return SendResponse(result);
        }

        [HttpGet("price")]
        [ProducesResponseType(typeof(ApiResponse<List<ConfigPrice>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllConfigPrice()
        {
            var result = await _configService.GetAllConfigPrice();
            return SendResponse(result);
        }

        [HttpPut("price")]
        [ProducesResponseType(typeof(ApiResponse<ConfigPrice>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateConfigPrice([FromBody] UpdateConfigPriceModel model)
        {
            var result = await _configService.UpdateConfigPrice(model);
            return SendResponse(result);
        }

        [HttpPost("price")]
        [ProducesResponseType(typeof(ApiResponse<ConfigPrice>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateConfigPrice([FromBody] CreateConfigPriceModel model)
        {
            var result = await _configService.CreateConfigPrice(model);
            return SendResponse(result);
        }

        [HttpDelete("price/{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteConfigPrice(Guid id)
        {
            var result = await _configService.DeleteConfigPrice(id);
            return SendResponse(result);
        }


        [HttpPost("price-reset")]
        [ProducesResponseType(typeof(ApiResponse<List<ConfigPrice>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetList([FromBody] List<CreateConfigPriceModel> model)
        {
            var result = await _configService.ResetPrice(model);
            return SendResponse(result);
        }
    }
}
