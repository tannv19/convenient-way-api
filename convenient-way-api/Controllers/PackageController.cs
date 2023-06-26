using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.ReportModel;
using ship_convenient.Services.PackageService;
using Swashbuckle.AspNetCore.Annotations;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly ILogger<PackageController> _logger;
        private readonly IPackageService _packageService;
        private readonly PackageSuggestService _packageSuggestService;
        public PackageController(ILogger<PackageController> logger, IPackageService packageService, PackageSuggestService packageSuggestService)
        {
            _logger = logger;
            _packageService = packageService;
            _packageSuggestService = packageSuggestService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list package")]
        [ProducesResponseType(typeof(ApiResponsePaginated<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(Guid? deliverId, Guid? senderId,string? id, string? status, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                ApiResponsePaginated<ResponsePackageModel> response = await _packageService.GetFilter(deliverId, senderId,id, status, pageIndex, pageSize);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get package by id")]
        [ProducesResponseType(typeof(ApiResponse<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetId(Guid id)
        {
            try
            {
                ApiResponse<ResponsePackageModel> response = await _packageService.GetById(id);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get list package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create pakage")]
        [ProducesResponseType(typeof(ApiResponse<ResponsePackageModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreatePackageModel model)
        {
            try
            {
                ApiResponse<ResponsePackageModel> response = await _packageService.Create(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Create package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("combos")]
        [SwaggerOperation(Summary = "Get suggest combos")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseComboPackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SuggestCombo(Guid deliverId)
        {
            try
            {
                /*ApiResponsePaginated<ResponseComboPackageModel> response = await _packageService.SuggestCombo(deliverId, pageIndex, pageSize);*/
                ApiResponse<List<ResponseComboPackageModel>> response = await _packageService.SuggestCombo(deliverId);
                return SendResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Api mapbox has exception : " + ex.Message);
                return StatusCode(500, "Api mapbox has exception : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get suggest combo has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("combos-v2")]
        [SwaggerOperation(Summary = "Get suggest combos")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseComboPackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SuggestComboV2(Guid deliverId)
        {
            try
            {
                /*ApiResponsePaginated<ResponseComboPackageModel> response = await _packageService.SuggestCombo(deliverId, pageIndex, pageSize);*/
                ApiResponse<List<ResponseComboPackageModel>> response = await _packageService.SuggestComboV2(deliverId);
                return SendResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Api mapbox has exception : " + ex.Message);
                return StatusCode(500, "Api mapbox has exception : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get suggest combo has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("suggest")]
        [SwaggerOperation(Summary = "Get suggest package not combo")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseSuggestPackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SuugestPackage(Guid deliverId)
        {
            try
            {
                ApiResponse<List<ResponseSuggestPackageModel>> response = await _packageSuggestService.SuggestCombo(deliverId);
                return SendResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Api mapbox has exception : " + ex.Message);
                return StatusCode(500, "Api mapbox has exception : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get suggest combo has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("suggest-v2")]
        [SwaggerOperation(Summary = "Get suggest package not combo")]
        [ProducesResponseType(typeof(ApiResponse<List<ResponseSuggestPackageModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SugestPackageV2(Guid deliverId)
        {
            try
            {
                ApiResponse<List<ResponseSuggestPackageModel>> response = await _packageSuggestService.SuggestComboV2(deliverId);
                return SendResponse(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Api mapbox has exception : " + ex.Message);
                return StatusCode(500, "Api mapbox has exception : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get suggest combo has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("approve")]
        [SwaggerOperation(Summary = "Approved pakage from status waiting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApprovedPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.ApprovedPackage(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Approve package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("reject")]
        [SwaggerOperation(Summary = "Reject pakage from status waiting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RejectPackage(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reject package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("deliver-selected")]
        [SwaggerOperation(Summary = "Shipper pickup a package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SelectedPackages(DeliverSelectedPackagesModel model)
        {
            try
            {
                ApiResponse response = await _packageService.DeliverSelectedPackages(model.deliverId,
                    model.packageIds);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper pickup package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("sender-cancel")]
        [SwaggerOperation(Summary = "Sender cancel package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SenderCancelPackage(CancelPackageModel model)
        {
            try
            {
                ApiResponse response = await _packageService.SenderCancelPackage(model.PackageId, model.Reason);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shop cancel package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("deliver-cancel")]
        [SwaggerOperation(Summary = "Sender cancel package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverCancelPackage(CancelPackageModel model)
        {
            try
            {
                ApiResponse response = await _packageService.DeliverCancelPackage(model.PackageId, model.Reason);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver cancel package exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("pickup-success")]
        [SwaggerOperation(Summary = "Deliver confirms packages and then delivery them")]
        
        [ProducesResponseType(typeof(ActionResult<ApiResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PickupPackageSuccess(Guid packageId)
        {
            try  
            {
                ApiResponse response = await _packageService.PickupPackageSuccess(
                    packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver confrims packages has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("confirm-success")]
        [SwaggerOperation(Summary = "Confirm sender is recevied packages")]

        [ProducesResponseType(typeof(ActionResult<ApiResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmSuccess(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.ToSuccessPackage(
                    packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver confrims packages has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("pickup-failed")]
        [SwaggerOperation(Summary = "Deliver confirms packages and then delivery them")]

        [ProducesResponseType(typeof(ActionResult<ApiResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PickupPackageFailed(PickupPackageFailedModel model)
        {
            try
            {
                ApiResponse response = await _packageService.PickupPackageFailed(
                    model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver confrims packages has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("delivered-failed")]
        [SwaggerOperation(Summary = "Shipper delivery failed package")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliveredFailedPackage(DeliveredFailedModel packageId)
        {
            try
            {
                ApiResponse response = await _packageService.DeliveredFailed(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper delivery failed package has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("delivered-success")]
        [SwaggerOperation(Summary = "Shipper delivery success pakage")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeliverDeliverySuccessPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.DeliveredSuccess(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Shipper delivery success pakage has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("refund-to-warehouse-success")]
        [SwaggerOperation(Summary = "Deliver refund package for sender success")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefundSuccessPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RefundToWarehouseSuccess(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver refund package for sender success has exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("refund-to-warehouse-failed")]
        [SwaggerOperation(Summary = "Deliver refund package for sender failed")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefundFailedPackage(Guid packageId)
        {
            try
            {
                ApiResponse response = await _packageService.RefundToWarehouseFailed(packageId);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deliver refund package for sender failed : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("report-problem")]
        [SwaggerOperation(Summary = "Report problem")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Report(CreateReportPackageModel model)
        {
            try
            {
                ApiResponse response = await _packageService.ReportProblem(model);
                return SendResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Report prolem exception : " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
