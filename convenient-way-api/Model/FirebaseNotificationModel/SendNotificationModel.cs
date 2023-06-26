using ship_convenient.Entities;
using System.Collections.Generic;

namespace ship_convenient.Model.FirebaseNotificationModel
{
    public class SendNotificationModel
    {
        public Guid AccountId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public IReadOnlyDictionary<string, string>? Data { get; set; }
        public string TypeOfNotification { get; set; } = string.Empty;

        public Notification ToEntity() {
            return new Notification()
            {
                AccountId = AccountId,
                Title = Title,
                Content = Body,
                TypeOfNotification = TypeOfNotification
            };
        }
    }
}
