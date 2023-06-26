using ship_convenient.Model.UserModel;

namespace ship_convenient.Entities
{
    public class ConfigUser : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public Guid InfoUserId { get; set; }
        public InfoUser? InfoUser { get; set; }

        public ResponseConfigUserModel ToResponseModel() {
            ResponseConfigUserModel model = new();
            model.Name = this.Name;
            model.Value = this.Value;
            model.ModifiedAt = this.ModifiedAt;
            model.InfoUserId = this.InfoUserId;
            return model;
        }
    }
}
