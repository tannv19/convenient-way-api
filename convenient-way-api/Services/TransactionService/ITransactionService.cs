using ship_convenient.Core.CoreModel;
using unitofwork_core.Model.TransactionModel;

namespace ship_convenient.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<ApiResponse<ResponseTransactionModel>> GetId(Guid id); 
        Task<ApiResponsePaginated<ResponseTransactionModel>> GetTransactions(Guid accountId,
            DateTime? from, DateTime? to, int pageIndex, int pageSize);
    }
}
