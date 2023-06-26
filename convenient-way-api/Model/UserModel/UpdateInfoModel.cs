using ship_convenient.Entities;

namespace ship_convenient.Model.UserModel
{
    public class UpdateInfoModel
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        public void UpdateEntity(Account account)
        {
            if (account.InfoUser != null) {
                account.InfoUser.FirstName = this.FirstName;
                account.InfoUser.LastName = this.LastName;
                account.InfoUser.Email = this.Email;
                account.InfoUser.Phone = this.Phone;
                account.InfoUser.PhotoUrl = this.PhotoUrl;
                account.InfoUser.Gender = this.Gender;
            }
        }
    }
}
