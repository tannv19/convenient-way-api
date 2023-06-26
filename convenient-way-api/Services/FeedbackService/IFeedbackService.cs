using ship_convenient.Core.CoreModel;
using ship_convenient.Model.FeedbackModel;
using ship_convenient.Model.UserModel;

namespace ship_convenient.Services.FeedbackService
{
    public interface IFeedbackService
    {
        Task<ApiResponse<ResponseFeedbackModel>> Create(CreateFeedbackModel model);
        Task<ApiResponse<ResponseFeedbackModel>> Update(UpdateFeedbackModel model);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponsePaginated<ResponseFeedbackModel>> GetList(Guid? packageId, Guid? creatorId,Guid? receiverId,
            string? feedbackFor, int pageIndex, int pageSize);
        Task<ApiResponse<RatingAccountModel>> GetRating(Guid accountId);
        Task<ApiResponse<RatingAccountModel>> GetRatingV2(Guid accountId);
    }
}
