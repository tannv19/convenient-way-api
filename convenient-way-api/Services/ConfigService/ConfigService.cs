using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigModel;
using ship_convenient.Services.GenericService;
using unitofwork_core.Constant.ConfigConstant;

namespace ship_convenient.Services.ConfigService
{
    public class ConfigService : GenericService<ConfigService>, IConfigService
    {
        private readonly IConfigPriceRepository _configPriceRepo;
        public ConfigService(ILogger<ConfigService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _configPriceRepo = unitOfWork.ConfigPrices;
        }

        public async Task<ApiResponse<ConfigPrice>> CreateConfigPrice(CreateConfigPriceModel model)
        {
            ApiResponse<ConfigPrice> response = new ApiResponse<ConfigPrice>();
            ConfigPrice configPrice = model.ToEntity();
            await _configPriceRepo.InsertAsync(configPrice);
            int result = _unitOfWork.Complete();
            if (result > 0)
            {
                response.ToSuccessResponse(configPrice, "Thêm mới thành công");
            }
            else
            {
                response.ToFailedResponse("Thêm mới thất bại");
            }
            return response;
        }

        public async Task<ApiResponse<List<ConfigPrice>>> CreateList(List<CreateConfigPriceModel> model)
        {
            ApiResponse<List<ConfigPrice>> response = new();
            List<ConfigPrice> configsPrice = model.Select((c) => c.ToEntity()).ToList();
            await _configPriceRepo.InsertAsync(configsPrice);
            int result = _unitOfWork.Complete();
            if (result > 0)
            {
                response.ToSuccessResponse(configsPrice, "Thêm mới thành công");
            }
            else
            {
                response.ToFailedResponse("Thêm mới thất bại");
            }
            return response;
        }

        public async Task<ApiResponse> DeleteConfigPrice(Guid id)
        {
            ApiResponse response = new();
            await _configPriceRepo.DeleteAsync(id);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse("Xóa cấu hình thành công");
            }
            else
            {
                response.ToFailedResponse("Có lỗi xảy ra");
            }
            return response;
        }

        public async Task<ApiResponse> DeleteList()
        {
            ApiResponse response = new();
            _configPriceRepo.DeleteRange(_configPriceRepo.GetAll());
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse("Xóa cấu hình thành công");
            }
            else
            {
                response.ToFailedResponse("Có lỗi xảy ra");
            }
            return response;
        }

        public async Task<ApiResponse<List<ResponseConfigModel>>> GetAll()
        {
            ApiResponse<List<ResponseConfigModel>> response = new();
            List<ConfigApp> configDefault = await _configRepo.GetAllAsync(predicate: (c) => c.Type == TypeOfConfig.DEFAULT);
            List<ConfigApp> configPrice = await _configRepo.GetAllAsync(predicate: (c) => c.Type == TypeOfConfig.PRICE_DISTANCE);

            List<ResponseConfigModel> responseConfigDefault = configDefault.Select((c) => c.ToResponseModel()).ToList();
            ResponseConfigModel? configCalculateWith = responseConfigDefault.FirstOrDefault(predicate: (c) => c.Name == ConfigConstant.CALCULATE_PRICE_WITH);
            if (configCalculateWith != null)
            {
                configCalculateWith.configsChildren = configPrice.Select((c) => c.ToResponseModel()).ToList();
            }
            response.ToSuccessResponse(responseConfigDefault, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<List<ConfigPrice>>> GetAllConfigPrice()
        {
            ApiResponse<List<ConfigPrice>> response = new();
            List<ConfigPrice> configsPrice = await _configPriceRepo.GetAllAsync();
            response.ToSuccessResponse(configsPrice, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<List<ConfigPrice>>> ResetPrice(List<CreateConfigPriceModel> model)
        {
            ApiResponse<List<ConfigPrice>> response = new();
            List<ConfigPrice> configsPrice = model.Select((c) => c.ToEntity()).ToList();
            _configPriceRepo.DeleteRange(_configPriceRepo.GetAll());
            await _configPriceRepo.InsertAsync(configsPrice);
            int result = _unitOfWork.Complete();
            if (result > 0)
            {
                response.ToSuccessResponse(configsPrice, "Cập nhật thành công");
            }
            else
            {
                response.ToFailedResponse("Cập nhật thất bại");
            }
            return response;
        }

        public async Task<ApiResponse<ConfigApp>> Update(UpdateConfigModel model)
        {
            ApiResponse<ConfigApp> response = new();
            ConfigApp? config = await _configRepo.FirstOrDefaultAsync((c) => c.Name == model.Name, disableTracking: false);
            if (config == null) {
                response.ToFailedResponse("Không tìm thấy thông tin cấu hình");
                return response;
            }
            config.Note = model.Note;
            config.ModifiedBy = model.ModifiedBy;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(config, "Cập nhật thông tin thành công");
            }
            else {
                response.ToFailedResponse("Cập nhật thông tin thất bại");
            }
            return response;
        }

        public async Task<ApiResponse<ConfigPrice>> UpdateConfigPrice(UpdateConfigPriceModel model)
        {
            ApiResponse<ConfigPrice> response = new();
            ConfigPrice config = model.ToEntity();
            _configPriceRepo.Update(config);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(config, "Cập nhật thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Cập nhật thông tin thất bại");
            }
            return response;
        }
    }
}
