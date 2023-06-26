using ship_convenient.Entities;

namespace ship_convenient.Model.UserModel
{
    public class ResponseAccountModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Balance { get; set; }
        public string Role { get; set; } = string.Empty;
        public string RegistrationToken { get; set; } = string.Empty;
        public ResponseInfoUserModel? InfoUser { get; set; }
    }
}
