using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper.SuggestPackageHelper;
using ship_convenient.Model.GoongModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;

namespace ship_convenient.Services.AccountService
{
    public class AccountService : GenericService<AccountService> ,IAccountService
    {
        private readonly IInfoUserRepository _infoUserRepo;
        private readonly AccountUtils _accountUtils;
        public AccountService(ILogger<AccountService> logger, IUnitOfWork unitOfWork, AccountUtils accountUtils)
            : base(logger, unitOfWork)
        {
            _accountUtils = accountUtils;
            _infoUserRepo = unitOfWork.InfoUsers;
        }

        public async Task<ApiResponse> IsCanCreate(VerifyValidAccountModel model)
        {
            ApiResponse response = new();
            #region verify params
            InfoUser? _checkPhone = await _infoUserRepo.FirstOrDefaultAsync(
                   predicate: (ac) => ac.Phone == model.Phone && model.Role == ac.Account.Role,
                   include: source => source.Include(ac => ac.Account));
            if (_checkPhone != null)
            {
                response.ToFailedResponse("Số điện thoại đã tồn tại, không thể đăng kí");
                return response;
            }
            Account? _checkUserName = await _accountRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.UserName == model.UserName && ac.Role == model.Role);
            if (_checkUserName != null)
            {
                response.ToFailedResponse("Tên đăng nhập đã tồn tại, không thể đăng kí");
                return response;
            }
            #endregion
            response.ToSuccessResponse("Thông tin hợp lệ để đăng kí");
            return response;
        }
        public async Task<ApiResponse<ResponseAccountModel>> Create(CreateAccountModel model)
        {
            ApiResponse<ResponseAccountModel> response = new();
            #region verify params
            if (model.IsCreateInfo()) {
                /*InfoUser? _checkEmail = await _infoUserRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.Email == model.Email);
                if (_checkEmail != null)
                {
                    response.ToFailedResponse("Email đã tồn tại, không thể đăng kí");
                    return response;
                }*/
                InfoUser? _checkPhone = await _infoUserRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.Phone == model.Phone && ac.Account.Role == model.Role,
                    include: source => source.Include(ac => ac.Account));
                if (_checkPhone != null)
                {
                    response.ToFailedResponse("Số điện thoại đã tồn tại, không thể đăng kí");
                    return response;
                }
            }
            Account? _checkUserName = await _accountRepo.FirstOrDefaultAsync(
                    predicate: (ac) => ac.UserName == model.UserName && ac.Role == model.Role);
            if (_checkUserName != null)
            {
                response.ToFailedResponse("Tên đăng nhập đã tồn tại, không thể đăng kí");
                return response;
            }
            #endregion
            Account account = model.ToEntityModel();
            account.Status = AccountStatus.NO_ROUTE;
            
            await _accountRepo.InsertAsync(account);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0) {
                response.ToSuccessResponse(account.ToResponseModel(), "Tạo tài khoản thành công");
            }
            else
            {
                response.ToFailedResponse("Tạo tài khoản thất bại");
            }
            return response;
        }
        
        public async Task<ApiResponse<ResponseAccountModel>> GetId(Guid id)
        {
            ApiResponse<ResponseAccountModel> response = new();
            #region Includable
            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> include = (acc) => acc.Include(a => a.InfoUser).ThenInclude(info => info != null ? info.Routes : null);
            #endregion
            Account? account = await _accountRepo.GetByIdAsync(id: id,
                include: include);
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            else
            {
                response.ToSuccessResponse(account.ToResponseModel(), " Lấy thông tin thành công");
            }
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseAccountModel>> 
            GetList(string? userName, string? status,string? role, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseAccountModel> response = new();
            #region Verify params
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return await Task.FromResult(response);
            }
            #endregion
            #region Includable
            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> include =
                (source) => source.Include(ac => ac.InfoUser);
            #endregion
            #region Predicates
            List<Expression<Func<Account, bool>>> predicates = new();
            if (!string.IsNullOrEmpty(userName))
            {
                Expression<Func<Account, bool>> predicateUserName = (ac) => ac.UserName.Contains(userName);
                predicates.Add(predicateUserName);
            }
            if (!string.IsNullOrEmpty(status))
            {
                Expression<Func<Account, bool>> predicateStatus = (ac) => ac.Status == status.ToUpper();
                predicates.Add(predicateStatus);
            }
            if (!string.IsNullOrEmpty(role))
            {
                string[] roles = role.Split(",");
                Expression<Func<Account, bool>> predicateRoles = (ac) => roles.Contains(ac.Role);
                predicates.Add(predicateRoles);
            }
            #endregion
            #region Order
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = (source) => source.OrderByDescending(tr => tr.CreatedAt);
            #endregion
            #region Selector
            Expression<Func<Account, ResponseAccountModel>> selector = (tran) => tran.ToResponseModel();
            #endregion
            PaginatedList<ResponseAccountModel> result = await _accountRepo.GetPagedListAsync(
                include: include,
                predicates: predicates, selector: selector, pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(result, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<ResponseAccountModel>> Update(UpdateAccountModel model)
        {
            ApiResponse<ResponseAccountModel> response = new();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (ac) => ac.Id == model.Id, disableTracking: false);
            #region verify params
            if (account == null) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
                return response;
            }
            #endregion
            account.Password = model.Password;
            account.Status = model.Status;
            account.Balance = model.Balance;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0) {
                response.ToSuccessResponse(account.ToResponseModel(), "Cập nhật thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Có lỗi xảy ra");
            }
            return response;
        }

        public async Task<ApiResponse<ResponseAccountModel>> UpdateInfo(UpdateInfoModel model)
        {
            ApiResponse<ResponseAccountModel> response = new();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (ac) => ac.Id == model.AccountId,
                include: (source) => source.Include(ac => ac.InfoUser)
                , disableTracking: false);
            #region verify params
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy tài khoản");
                return response;
            }
            #endregion
            model.UpdateEntity(account);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(account.ToResponseModel(), "Cập nhật thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Có lỗi xảy ra");
            }
            return response;
        }

        public async Task<ApiResponse> UpdateRegistrationToken(UpdateTokenModel model)
        {
            ApiResponse repsonse = new ApiResponse();
            Account? account = await _accountRepo.FirstOrDefaultAsync(
                predicate: (ac) => ac.Id == model.AccountId, disableTracking: false);

            if (account == null) {
                repsonse.ToFailedResponse("Không tìm thấy tài khoản");
                return repsonse;
            }
            account.RegistrationToken = model.RegistrationToken;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                repsonse.ToSuccessResponse("Cập nhật thông tin thành công");
            }
            else
            {
                repsonse.ToFailedResponse("Có lỗi xảy ra");
            }
            return repsonse;
        }

        public async Task<ApiResponse<ResponseBalanceModel>> AvailableBalance(Guid accountId)
        {
            ApiResponse<ResponseBalanceModel> response = new();
            response.ToSuccessResponse(await _accountUtils.AvailableBalanceModel(accountId),"Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<List<DistancePackageModel>>> GetOrderPointVirtual(Guid accountId)
        {
            ApiResponse<List<DistancePackageModel>> response = new();
            List<DistancePackageModel> result = await _accountUtils.GetCurrentVirtualPoint(accountId);
            if (result.Count > 0) {
                response.ToSuccessResponse(result, "Lấy thông tin thành công");
            }
            else
            {
                response.ToFailedResponse("Không tìm thấy lộ trình ảo");
            }
            return response;
        }

        public async Task<ApiResponse<List<ResponseSearchModel>>> GetHistoryLocation(Guid senderId)
        {
            ApiResponse<List<ResponseSearchModel>> response = new();
            List<Package> packages = await _packageRepo.GetAllAsync(
                predicate: p => p.SenderId == senderId);
            List<ResponseSearchModel> result = new();
            for (int i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                ResponseSearchModel startPoint = new ResponseSearchModel();
                startPoint.Name = package.StartAddress;
                startPoint.Longitude = package.StartLongitude;
                startPoint.Latitude = package.StartLatitude;

                if (!result.Any(location => location.Name == startPoint.Name)) {
                    result.Add(startPoint);
                }

                ResponseSearchModel endPoint = new ResponseSearchModel();
                endPoint.Name = package.DestinationAddress;
                endPoint.Longitude = package.DestinationLongitude;
                endPoint.Latitude = package.DestinationLatitude;

                if (!result.Any(location => location.Name == endPoint.Name))
                {
                    result.Add(endPoint);
                }
            }
            response.ToSuccessResponse(result, "Lấy thông tin thành công");
            return response;
        }
    }
}
