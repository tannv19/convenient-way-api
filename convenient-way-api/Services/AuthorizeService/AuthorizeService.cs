using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Helper;
using ship_convenient.Model.AuthorizeModel;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;

namespace ship_convenient.Services.AuthorizeService
{
    public class AuthorizeService : GenericService<AuthorizeService>, IAuthorizeService
    {
        private readonly IConfiguration _configuration;
        public AuthorizeService(ILogger<AuthorizeService> logger, IUnitOfWork unitOfWork, IConfiguration configuration) : base(logger, unitOfWork)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<ResponseLoginModel>> Login(LoginModel model)
        {
            ApiResponse<ResponseLoginModel> response = new ApiResponse<ResponseLoginModel>();

            Func<IQueryable<Account>, IIncludableQueryable<Account, object?>> include = (acc) => acc.Include(a => a.InfoUser).ThenInclude(info => info != null ? info.Routes : null);
            Expression<Func<Account, bool>> predicate = (acc) => 
                (acc.UserName == model.UserName || acc.InfoUser.Phone == model.UserName) && acc.Password == model.Password && acc.Role == model.Role;
            Account? account = await _accountRepo.FirstOrDefaultAsync(predicate: predicate, include: include, disableTracking: false);
            if (account != null)
            {
                ResponseLoginModel rsToken = new ResponseLoginModel();
                rsToken.Account = account.ToResponseModel();
                rsToken.Token = JWTHelper.GenerateJWTToken(account, _configuration["JWT:Key"]);
                response.ToSuccessResponse(rsToken, "Đăng nhập thành công");

                if (!string.IsNullOrEmpty(model.RegistrationToken)) {
                    account.RegistrationToken = model.RegistrationToken;
                    await _unitOfWork.CompleteAsync();
                }
            }
            else
            {
                response.ToFailedResponse("Tên tài khoản hoặc mật khẩu không đúng");
            }
            return response;

        }

        public Task<ApiResponse<ResponseLoginModel>> Login(LoginFirebaseModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> LogOut(Guid accountId)
        {
            ApiResponse response = new();
            Account? account = _accountRepo.GetById(accountId);
            if (account != null)
            {
                account.RegistrationToken = string.Empty;
                _accountRepo.Update(account);
                await _unitOfWork.CompleteAsync();
            }
            response.ToSuccessResponse("Đăng xuất thành công");
            return response;
        }
    }
}
