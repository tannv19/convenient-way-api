namespace ship_convenient.Model.UserModel
{
    public class ResponseConfigUserModel
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public Guid InfoUserId { get; set; }
    }
}
