namespace ship_convenient.Model.FeedbackModel
{
    public class UpdateFeedbackModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }

        
    }
}
