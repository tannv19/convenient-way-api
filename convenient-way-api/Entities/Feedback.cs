using ship_convenient.Model.FeedbackModel;

namespace ship_convenient.Entities
{
    public class Feedback : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string FeedbackFor { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        public Guid CreatorId { get; set; }
        public Account? Creator { get; set; }
        public Guid ReceiverId { get; set; }
        public Account? Receiver { get; set; }


        public ResponseFeedbackModel ToResponseModel()
        {
            ResponseFeedbackModel model = new();
            model.Id = this.Id;
            model.Content = this.Content;
            model.Rating = this.Rating;
            model.FeedbackFor = this.FeedbackFor;
            model.PackageId = this.PackageId;
            model.Package = this.Package?.ToResponseModel();
            model.CreatorId = this.CreatorId;
            model.Creator = this.Creator?.ToResponseModel();
            model.ReceiverId = this.ReceiverId;
            model.Receiver = this.Receiver?.ToResponseModel();
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            return model;
        }
    }
}
