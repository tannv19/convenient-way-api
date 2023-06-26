using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.ReportModel;
using ship_convenient.Services.GenericService;
using System.Linq.Expressions;

namespace ship_convenient.Services.ReportService
{
    public class ReportService : GenericService<ReportService>, IReportService
    {
        private readonly IReportRepository _reportRepo;
        public ReportService(ILogger<ReportService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _reportRepo = unitOfWork.Reports;
        }

        public async Task<ApiResponsePaginated<ResponseReportModel>> GetList(Guid? creatorId,Guid? receiverId, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseReportModel> response = new();
            #region Verify params
            if (pageIndex < 0 || pageSize < 1)
            {
                response.ToFailedResponse("Thông tin phân trang không hợp lệ");
                return response;
            }
            #endregion

            #region Includable
            Func<IQueryable<Report>, IIncludableQueryable<Report, object?>> include = (source) => source.Include(r => r.Package)
                .Include(r => r.Creator).ThenInclude(a => a.InfoUser)
                .Include(r => r.Receiver).ThenInclude(a => a.InfoUser);
            #endregion

            #region Predicates
            List<Expression<Func<Report, bool>>> predicates = new List<Expression<Func<Report, bool>>>();
            if (creatorId != null)
            {
                Expression<Func<Report, bool>> filterAccount = (p) => p.CreatorId == creatorId;
                predicates.Add(filterAccount);
            }
            if (receiverId != null)
            {
                Expression<Func<Report, bool>> filterAccount = (p) => p.ReceiverId == receiverId;
                predicates.Add(filterAccount);
            }

            #endregion

            #region Order
            Func<IQueryable<Report>, IOrderedQueryable<Report>> orderBy = (source) => source.OrderBy(p => p.CreatedAt);
            #endregion

            Expression<Func<Report, ResponseReportModel>> selector = (report) => report.ToResponseModel();
            PaginatedList<ResponseReportModel> items;
            items = await _reportRepo.GetPagedListAsync(
                 selector: selector, include: include, predicates: predicates,
                 orderBy: orderBy, pageIndex: pageIndex, pageSize: pageSize);
            _logger.LogInformation("Total count: " + items.TotalCount);
            #region Response result
            response.SetData(items);
            int countPackage = items.Count;
            if (countPackage > 0)
            {
                response.Message = "Lấy thông tin thành công";
            }
            else
            {
                response.Message = "Không tìm thấy sự cố";
            }
            #endregion
            return response;
        }
    }
}
