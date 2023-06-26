using ship_convenient.Model.UserModel;

namespace ship_convenient.Entities
{
    public class Account : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Balance { get; set; }
        public string Role { get; set; } = string.Empty;
        public string RegistrationToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        #region Relationship
        public Guid InfoUserId { get; set; }
        public InfoUser? InfoUser { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Package> PackageSenders { get; set; }
        public List<Package> PackageDelivers { get; set; }
        public List<Feedback> CreatorFeedbacks { get; set; }
        public List<Feedback> ReceiverFeedbacks { get; set; }
        public List<Report> CreatorReports { get; set; }
        public List<Report> ReceiverReports { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Deposit> Deposits { get; set; }
        #endregion

        public Account()
        {
            Notifications = new List<Notification>();
            PackageSenders = new List<Package>();
            PackageDelivers = new List<Package>();
            Transactions = new List<Transaction>();
            Deposits = new List<Deposit>();
            CreatorFeedbacks = new List<Feedback>();
            ReceiverFeedbacks = new List<Feedback>();
            CreatorReports = new List<Report>();
            ReceiverReports = new List<Report>();
        }

        public ResponseAccountModel ToResponseModel()
        {
            ResponseAccountModel model = new();
            model.Id = this.Id;
            model.UserName = this.UserName;
            model.Status = this.Status;
            model.Role = this.Role;
            model.Balance = this.Balance;
            model.RegistrationToken = this.RegistrationToken;
            if (InfoUser != null)
            {
                model.InfoUser = this.InfoUser.ToResponseModel();
            }
            return model;
        }

        public string GetFullName() {
            return (this.InfoUser?.LastName + " " + this.InfoUser?.FirstName).Trim();
        }
    }
}
