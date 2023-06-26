using ship_convenient.Entities;

namespace ship_convenient.Model.FeedbackModel
{
    public class CreateFeedbackModel
    {
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string FeedbackFor { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public Guid CreatorId { get; set; }
        public Guid ReceiverId { get; set; }

        public Feedback ToEntity() {
            Feedback feedback = new Feedback();
            feedback.Content = Content;
            feedback.Rating = Rating;
            feedback.FeedbackFor = FeedbackFor;
            feedback.PackageId = PackageId;
            feedback.CreatorId = CreatorId;
            feedback.ReceiverId = ReceiverId;
            return feedback;
        }
    }
}
