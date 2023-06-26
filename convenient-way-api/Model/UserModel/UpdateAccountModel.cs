namespace ship_convenient.Model.UserModel
{
    public class UpdateAccountModel
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Balance { get; set; }
    }
}
