namespace ship_convenient.Model.AuthorizeModel
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RegistrationToken { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
