using Microsoft.EntityFrameworkCore;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.ConfigUserModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.ConfigUserService
{
    public class ConfigUserService : GenericService<ConfigUserService>, IConfigUserService
    {
        private readonly IInfoUserRepository _infoUserRepo;
        public ConfigUserService(ILogger<ConfigUserService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _infoUserRepo = _unitOfWork.InfoUsers;
        }

        public async Task<ApiResponse<List<ResponseConfigUserModel>>> GetList(Guid accountId)
        {
            ApiResponse<List<ResponseConfigUserModel>> response = new();
            Account? account = await _accountRepo.GetByIdAsync(accountId, include: source => source.Include(ac => ac.InfoUser)
                        .ThenInclude(info => info.ConfigUsers));
            List<ResponseConfigUserModel>? configs = account?.InfoUser?.ConfigUsers.Select(c => c.ToResponseModel()).ToList();
            if (configs != null)
            {
                response.ToSuccessResponse(configs, "Lấy thông tin thàn công");
            }
            else {
                response.ToFailedResponse("Lấy thông tin thất bại");
            }
            return response;
        }

        public async Task<ApiResponse<ResponseConfigUserModel>> Update(UpdateUserConfigModel model)
        {
            ApiResponse<ResponseConfigUserModel> response = new();
            Account? account = await _accountRepo.GetByIdAsync(model.AccountId, include: source => source.Include(ac => ac.InfoUser)
                        .ThenInclude(info => info.ConfigUsers), disableTracking: false);
            ConfigUser? config = account?.InfoUser?.ConfigUsers.Where(config => config.Name == model.ConfigName).FirstOrDefault();
            if (config != null) config.Value = model.ConfigValue;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0) {
                response.ToSuccessResponse(config.ToResponseModel(), "Cập nhật thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Cập nhật thông tin thất bại");
            }
            return response;
        }

        public async Task<ApiResponse> UpdateLocationUser(UpdateLocationUserModel model)
        {
            ApiResponse response = new();
            InfoUser? info = await _infoUserRepo.FirstOrDefaultAsync(
                predicate: (info) => info.AccountId == model.AccountId, disableTracking: false);
            if (info != null)
            {
                info.Latitude = model.Latitude;
                info.Longitude = model.Longitude;
                int result = await _unitOfWork.CompleteAsync();
                if (result > 0) {
                    response.ToSuccessResponse("Cập nhật tọa độ thành công");
                }
                else
                {
                    response.ToFailedResponse("Yêu cầu không thành công");
                }
            }
            else {
                response.ToFailedResponse("Yêu cầu không thành công");
            }
            return response;
        }
    }
}
