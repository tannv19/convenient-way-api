using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.Repository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;
using unitofwork_core.Model.TransactionModel;
using System;

namespace ship_convenient.Services.TransactionService
{
    public class TransactionService : GenericService<TransactionService>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        public TransactionService(ILogger<TransactionService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _transactionRepo = unitOfWork.Transactions;
            _accountRepo = unitOfWork.Accounts;
        }

        public async Task<ApiResponse<ResponseTransactionModel>> GetId(Guid id)
        {
            ApiResponse<ResponseTransactionModel> response = new();
            Transaction? tran = await _transactionRepo.GetByIdAsync(id);
            if (tran == null)
            {
                response.ToFailedResponse("Không tìm thấy giao dịch");
            }
            else
            {
                response.ToSuccessResponse(tran.ToResponseModel(), "Lấy thông tin thành công");
            }
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseTransactionModel>> GetTransactions(Guid accountId, DateTime? from, DateTime? to, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseTransactionModel> response = new ApiResponsePaginated<ResponseTransactionModel>();
            #region Verify params
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return await Task.FromResult(response);
            }
            #endregion
            #region Predicates
            List<Expression<Func<Transaction, bool>>> predicates = new();
            if (accountId != Guid.Empty)
            {
                Expression<Func<Transaction, bool>> predicateAccount =
                    (trans) => trans.AccountId == accountId;
                predicates.Add(predicateAccount);
            }
            if (from != null && to != null)
            {
                bool isValidDate = from <= to;
                if (!isValidDate)
                {
                    response.ToFailedResponse("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
                    return response;
                }
            }
            if (from != null)
            {
                Expression<Func<Transaction, bool>> predicateDateTime = (transaction) =>
                    transaction.CreatedAt >= from;
                predicates.Add(predicateDateTime);
            }
            if (to != null)
            {
                Expression<Func<Transaction, bool>> predicateDateTime2 = (transaction) =>
                  transaction.CreatedAt <= to.Value.AddHours(23).AddMinutes(59);
                predicates.Add(predicateDateTime2);
            }
            #endregion
            #region Order
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = (source) => source.OrderByDescending(tr => tr.CreatedAt);
            #endregion
            #region Selector
            Expression<Func<Transaction, ResponseTransactionModel>> selector = (tran) => tran.ToResponseModel();
            #endregion
            PaginatedList<ResponseTransactionModel> result = await _transactionRepo.GetPagedListAsync<ResponseTransactionModel>(
                predicates: predicates, orderBy: orderBy, selector: selector, 
                pageIndex: pageIndex, pageSize: pageSize);
            response.SetData(result, "Lấy thông tin thành công");
            return response;
        }
    }
}
