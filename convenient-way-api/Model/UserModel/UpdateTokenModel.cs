namespace ship_convenient.Model.UserModel
{
    public class UpdateTokenModel
    {
        public Guid AccountId { get; set; }
        public string RegistrationToken { get; set; } = string.Empty;
    }
}
