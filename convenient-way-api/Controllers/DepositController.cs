using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.DepositModel;
using ship_convenient.Services.DepositService;

namespace ship_convenient.Controllers
{

    public class DepositController : BaseApiController
    {
        private readonly IDepositService _depositService;
        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ResponseDepositModel>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDeposit(Guid id)
        {
            ApiResponse<ResponseDepositModel> response = await _depositService.GetId(id);
            return SendResponse(response);
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ApiResponse<ResponseDepositModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDeposit(CreateDepositModel model)
        {
            ApiResponse<ResponseDepositModel> response = await _depositService.Create(model);
            return SendResponse(response);
        }
    }
}
