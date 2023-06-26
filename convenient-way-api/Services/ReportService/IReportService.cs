using ship_convenient.Core.CoreModel;
using ship_convenient.Model.ReportModel;

namespace ship_convenient.Services.ReportService
{
    public interface IReportService
    {
        Task<ApiResponsePaginated<ResponseReportModel>> GetList(Guid? creatorId,Guid? receiverId, int page, int pageSize);
    }
}
