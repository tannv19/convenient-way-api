using ship_convenient.Entities;
using ship_convenient.Model.UserModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Model.FeedbackModel
{
    public class ResponseFeedbackModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string FeedbackFor { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public ResponsePackageModel? Package { get; set; }
        public Guid CreatorId { get; set; }
        public ResponseAccountModel? Creator { get; set; }
        public Guid ReceiverId { get; set; }
        public ResponseAccountModel? Receiver { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
