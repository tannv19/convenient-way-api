namespace ship_convenient.Model.NotificationModel
{
    public class ResponseNotificationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsSend { get; set; } = false;
        public string TypeOfNotification { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        #region Relationship
        public Guid AccountId { get; set; }
        #endregion
    }
}
