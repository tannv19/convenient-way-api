using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.DepositModel;
using ship_convenient.Services.GenericService;

namespace ship_convenient.Services.DepositService
{
    public class DepositService : GenericService<DepositService>, IDepositService
    {
        private readonly IDepositRepository _depositRepo;
        private readonly IAccountRepository _accountRepo;
        public DepositService(ILogger<DepositService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _depositRepo = unitOfWork.Deposits;
            _accountRepo = unitOfWork.Accounts;
        }

        public async Task<ApiResponse<ResponseDepositModel>> Create(CreateDepositModel model)
        {
            ApiResponse<ResponseDepositModel> response = new();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (i) => i.Id == model.AccountId, disableTracking: false, include: (en) => en.Include(info => info.InfoUser));

            #region verify params
            if (account == null)
            {
                response.ToFailedResponse("Thông tin người dùng không tồn tại");
                return response;
            }
            #endregion
            Deposit deposit = model.ToEntity();
            await _depositRepo.InsertAsync(deposit);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(deposit.ToResponseModel(), "Tạo nạp tiền thành công");
            }
            else
            {
                response.ToFailedResponse("Tạo nạp tiền thất bại");
            }
            return response;

        }

        public async Task<ApiResponse<ResponseDepositModel>> GetId(Guid id)
        {
            ApiResponse<ResponseDepositModel> response = new();
            #region Includable
            Func<IQueryable<Deposit>, IIncludableQueryable<Deposit, object?>> include = (acc) => acc.Include(a => a.Account);
            #endregion
            Deposit? deposit = await _depositRepo.GetByIdAsync(id: id,
                include: include);
            if (deposit == null)
            {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            else
            {
                response.ToSuccessResponse(deposit.ToResponseModel(), " Lấy thông tin thành công");
            }
            return response;
        }
    }
}
