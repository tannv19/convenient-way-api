using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ship_convenient.Constants.AccountConstant;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.FeedbackModel;
using ship_convenient.Model.UserModel;
using ship_convenient.Services.GenericService;
using System;
using System.Linq.Expressions;

namespace ship_convenient.Services.FeedbackService
{
    public class FeedbackService : GenericService<FeedbackService>, IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;
        public FeedbackService(ILogger<FeedbackService> logger, IUnitOfWork unitOfWork) : base(logger, unitOfWork)
        {
            _feedbackRepo = unitOfWork.Feedbacks;
        }

        public async Task<ApiResponse<ResponseFeedbackModel>> Create(CreateFeedbackModel model)
        {
            ApiResponse<ResponseFeedbackModel> response = new();
            #region verify params
            string? errorRating = verifyRating(model.Rating);
            if (!string.IsNullOrEmpty(errorRating))
            {
                response.ToFailedResponse(errorRating);
                return response;
            }
            if (!IsExistedPackage(model.PackageId))
            {
                response.ToFailedResponse("Không tìm thấy gói hàng");
                return response;
            }
            Feedback? FeedbackForRole = await _feedbackRepo.FirstOrDefaultAsync(predicate: (fb) => fb.FeedbackFor == model.FeedbackFor
                            && fb.PackageId == model.PackageId);
            if (FeedbackForRole != null) {
                response.ToFailedResponse($"Phản hồi đã tồn tại dành cho {model.FeedbackFor}");
                return response;
            }
            #endregion
            Feedback feedback = model.ToEntity();
            await _feedbackRepo.InsertAsync(feedback);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(feedback.ToResponseModel(), "Tạo phản hồi thành công");
            }
            else
            {
                response.ToFailedResponse("Không thể phản hồi");
            }
            return response;
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            ApiResponse response = new();
            await _feedbackRepo.DeleteAsync(id);
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse("Xóa phản hồi thành công");
            }
            else
            {
                response.ToFailedResponse("Không thể xóa phản hồi");
            }
            return response;
        }

        public async Task<ApiResponsePaginated<ResponseFeedbackModel>> GetList(Guid? packageId, Guid? creatorId, Guid? receiverId,
            string? feedbackFor, int pageIndex, int pageSize)
        {
            ApiResponsePaginated<ResponseFeedbackModel> response = new();
            #region Verify params
            string? errorPaging = VerifyPaging(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(errorPaging))
            {
                response.ToFailedResponse(errorPaging);
                return response;
            }
            #endregion
            #region Predicates
            List<Expression<Func<Feedback, bool>>> predicates = new();
            if (packageId != null)
            {
                Expression<Func<Feedback, bool>> filterPackage = (feed) => feed.PackageId.Equals(packageId);
                predicates.Add(filterPackage);
            }
            if (creatorId != null)
            {
                Expression<Func<Feedback, bool>> filterAccount = (account) => account.CreatorId.Equals(creatorId);
                predicates.Add(filterAccount);
            }
            if (receiverId != null)
            {
                Expression<Func<Feedback, bool>> filterAccount = (account) => account.ReceiverId.Equals(receiverId);
                predicates.Add(filterAccount);
            }
            if (!string.IsNullOrEmpty(feedbackFor))
            {
                Expression<Func<Feedback, bool>> filterFor = (fb) => fb.FeedbackFor.Equals(feedbackFor);
                predicates.Add(filterFor);
            }
            #endregion
            #region Include
            Func<IQueryable<Feedback>, IIncludableQueryable<Feedback, object?>> include = (source) => source.Include(feed => feed.Package);
            #endregion
            #region Order
            Func<IQueryable<Feedback>, IOrderedQueryable<Feedback>> orderBy = (source) => source.OrderByDescending(fe => fe.CreatedAt);
            #endregion
            #region Selector
            Expression<Func<Feedback, ResponseFeedbackModel>> selector = (feed) => feed.ToResponseModel();
            #endregion
            PaginatedList<ResponseFeedbackModel> result = await _feedbackRepo.GetPagedListAsync(
                include: include,
                predicates: predicates, orderBy: orderBy, selector: selector,
                pageSize: pageSize, pageIndex: pageIndex);
            response.SetData(result, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<RatingAccountModel>> GetRating(Guid accountId)
        {
            ApiResponse<RatingAccountModel> response = new();
            Account? account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy thông tin tài khoản");
                return response;
            }
            #region Includeable
            Func<IQueryable<Feedback>, IIncludableQueryable<Feedback, object?>> includeable = (source) => 
            source.Include(feed => feed.Creator).Include(feed => feed.Receiver)
                .Include(feed => feed.Package);
            #endregion
            #region Predicates
            Expression<Func<Feedback, bool>> filterSenderRole = (feed) =>
                feed.Package!.SenderId.Equals(accountId) && feed.FeedbackFor == FeedbackFor.SENDER;
            Expression<Func<Feedback, bool>> filterDeliverRole = (feed) =>
              feed.Package!.DeliverId.Equals(accountId) && feed.FeedbackFor == FeedbackFor.DELIVER;
            #endregion
            List<Feedback> feedbackOfSendersRole = await _feedbackRepo.GetAllAsync(include: includeable, predicate: filterSenderRole);
            List<Feedback> feedbackOfDeliversRole = await _feedbackRepo.GetAllAsync(include: includeable, predicate: filterDeliverRole);
            int totalRatingSender = feedbackOfSendersRole.Count;
            int totalRatingDeliver = feedbackOfDeliversRole.Count;
            double averageRatingSender = feedbackOfSendersRole.Sum(feed => feed.Rating) / totalRatingSender;
            double averageRatingDeliver = feedbackOfDeliversRole.Sum(feed => feed.Rating) / totalRatingDeliver;
            RatingAccountModel model = new()
            {
                TotalRatingSender = totalRatingSender,
                AverageRatingSender = averageRatingSender,
                TotalRatingDeliver = totalRatingDeliver,
                AverageRatingDeliver = averageRatingDeliver
            };
            response.ToSuccessResponse(model, "Lấy thông tin thành công");
            return response;
        }

        public async Task<ApiResponse<RatingAccountModel>> GetRatingV2(Guid accountId)
        {
            ApiResponse<RatingAccountModel> response = new();
            Account? account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null)
            {
                response.ToFailedResponse("Không tìm thấy thông tin tài khoản");
                return response;
            }
            RatingAccountModel model = new();
            if (account.Role == RoleName.SENDER)
            {
                List<Feedback> feedbacks = await _feedbackRepo.GetAllAsync(predicate: (feed) => feed.ReceiverId.Equals(accountId));
                model.TotalRatingSender = feedbacks.Count;
                model.AverageRatingSender = feedbacks.Sum(feed => feed.Rating) / model.TotalRatingSender;
            }
            else {
                List<Feedback> feedbacks = await _feedbackRepo.GetAllAsync(predicate: (feed) => feed.ReceiverId.Equals(accountId));
                model.TotalRatingDeliver = feedbacks.Count;
                model.AverageRatingDeliver = feedbacks.Sum(feed => feed.Rating) / model.TotalRatingDeliver;
            }
            response.ToSuccessResponse(model, "Lấy thông tin thành công");
            return response;

        }

        public async Task<ApiResponse<ResponseFeedbackModel>> Update(UpdateFeedbackModel model)
        {
            ApiResponse<ResponseFeedbackModel> response = new();
            Feedback? feedback = await _feedbackRepo.GetByIdAsync(model.Id, disableTracking: false);
            #region Verify params
            if (feedback == null)
            {
                response.ToFailedResponse("Phản hồi không tồn tại");
                return response;
            }
            string? errorRating = verifyRating(model.Rating);
            if (!string.IsNullOrEmpty(errorRating))
            {
                response.ToFailedResponse(errorRating);
                return response;
            }
            #endregion
            feedback.Content = model.Content;
            feedback.Rating = model.Rating;
            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                response.ToSuccessResponse(feedback.ToResponseModel(), "Cập nhật phản hồi thành công");
            }
            else
            {
                response.ToFailedResponse("Cập nhật phản hồi thất bại");
            }
            return response;
        }

        private string? verifyRating(double rating)
        {
            string? result = null;
            if (rating <= 0) result = "Phản hồi ít nhất phải có 1 sao!!!";

            if (rating > 5) result = "Phản hồi có tối đa là 5 sao!!!";

            if (rating <= 0 && rating > 5) result = "Phản hồi có ít nhất là 1 sao và tối đa là 5 sao !!!";
            return result;
        }
    }
}
