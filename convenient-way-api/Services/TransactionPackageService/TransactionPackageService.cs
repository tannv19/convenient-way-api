using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.PackageModel;
using ship_convenient.Model.TransactionPackageModel;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using unitofwork_core.Constant.Package;

namespace ship_convenient.Services.TransactionPackageService
{
    public class TransactionPackageService : GenericService<TransactionPackageService>, ITransactionPackageService
    {
        private readonly ITransactionPackageRepository _transactionsPackageRepo;
        public TransactionPackageService(ILogger<TransactionPackageService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _transactionsPackageRepo = unitOfWork.TransactionPackages;
        }

        public async Task<ApiResponsePaginated<ResponseCancelPackageModel>> GetCancelPackages(Guid? deliverId, Guid? senderId, string status, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseCancelPackageModel> response = new();
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (errorPaging != null)
            {
                response.ToFailedResponse(errorPaging);
                return response;
            }

            #region Predicate
            List<Expression<Func<Package, bool>>> predicates = new();
            if (deliverId != null)
            {
                predicates.Add((source) => source.DeliverId == deliverId);
            }
            if (senderId != null)
            {
                predicates.Add((source) => source.SenderId == senderId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                predicates.Add((source) => source.Status == status);
            }
            #endregion
            #region Includable
            Func<IQueryable<Package>, IIncludableQueryable<Package, object?>> include = (source) => source.Include(p => p.TransactionPackages)
                                        .Include(p => p.Products)
                                        .Include(p => p.Sender).ThenInclude(a => a != null ? a.InfoUser : null)
                                        .Include(p => p.Deliver).ThenInclude(a => a != null ? a.InfoUser : null);
            #endregion
            #region Selector
            Expression<Func<Package, ResponseCancelPackageModel>> selector;
            if (status == PackageStatus.SENDER_CANCEL)
            {
                selector = (source) => source.ToSenderCancelPackage();
            }
            else // if(status == PackageStatus.DELIVER_CANCEL)
            {
                selector = (source) => source.ToDeliverCancelPackage();
            }
            #endregion
            #region OrderBy
            Func<IQueryable<Package>, IOrderedQueryable<Package>> orderBy = (source) => source.OrderByDescending(p => p.ModifiedAt);
            #endregion
            PaginatedList<ResponseCancelPackageModel> packages = await _packageRepo.GetPagedListAsync(
                    predicates: predicates,
                    include: include,
                    selector: selector,
                    orderBy: orderBy,
                    pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(packages, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseCancelPackageModel>> GetDeliverCancelPackage(Guid deliverId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseCancelPackageModel> response = new();
            Account? account = await _accountRepo.GetByIdAsync(deliverId);
            #region Verify params
            if (account == null) {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (errorPaging != null) {
                response.ToFailedResponse(errorPaging);
                return response; 
            }
            #endregion
            PaginatedList<ResponseCancelPackageModel> packages = await _packageRepo.GetPagedListAsync(
                    predicate: (source) => source.DeliverId == deliverId && source.Status == PackageStatus.DELIVER_CANCEL,
                    include: (source) => source.Include(p => p.TransactionPackages),
                    selector: (source) => source.ToDeliverCancelPackage(),
                    orderBy: (source) => source.OrderByDescending(p => p.ModifiedAt),
                    pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(packages, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseTransactionPackageModel>> GetHistoryPackage(Guid packageId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseTransactionPackageModel> response = new();

            #region Verify params
            Package? package = await _packageRepo.GetByIdAsync(packageId);
            if (package == null)
            {
                response.ToFailedResponse("Gói hàng không tồn tại");
            }
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return response;
            }
            #endregion

            #region Predicate
            Expression<Func<TransactionPackage, bool>> predicate = (source) => source.PackageId == packageId;
            #endregion
            #region Order
            Func<IQueryable<TransactionPackage>, IOrderedQueryable<TransactionPackage>> orderBy = (source) => source.OrderByDescending(p => p.CreatedAt);
            #endregion
            #region Selector
            Expression<Func<TransactionPackage, ResponseTransactionPackageModel>> selector = (source) => source.ToResponseModel();
            #endregion
            PaginatedList<ResponseTransactionPackageModel> items = await _transactionsPackageRepo.GetPagedListAsync(predicate: predicate, orderBy: orderBy,
                selector: selector, pageIndex: pageIndex, pageSize: pageSize);
            if (items.Count > 0)
            {
                response.SetData(items, "Thông tin lịch sử của gói hàng");
            }
            else
            {
                response.Message = "Không có thông tin lịch sử của gói hàng";
            }
            return response;
        }

        

        public async Task<ApiResponsePaginated<ResponseCancelPackageModel>> GetSenderCancelPackage(Guid senderId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseCancelPackageModel> response = new();
            Account? account = await _accountRepo.GetByIdAsync(senderId);
            #region Verify params
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy tài khoản");
            }
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (errorPaging != null)
            {
                response.ToFailedResponse(errorPaging);
                return response;
            }
            #endregion
            PaginatedList<ResponseCancelPackageModel> packages = await _packageRepo.GetPagedListAsync(
                    predicate: (source) => source.SenderId == senderId && source.Status == PackageStatus.SENDER_CANCEL,
                    include: (source) => source.Include(p => p.TransactionPackages),
                    selector: (source) => source.ToSenderCancelPackage(),
                    orderBy: (source) => source.OrderByDescending(p => p.ModifiedAt),
                    pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(packages, "Lấy thông tin thành công");
            return response;
        }
    }
}
