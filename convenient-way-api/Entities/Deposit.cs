using ship_convenient.Model.DepositModel;

namespace ship_convenient.Entities
{
    public class Deposit : BaseEntity
    {
        public int Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionIdPartner { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        #region Relationship
        public List<Transaction> Transactions { get; set; }
        #endregion
        public Deposit()
        {
            Transactions = new();
        }

        public ResponseDepositModel ToResponseModel()
        {
            ResponseDepositModel model = new ResponseDepositModel();
            model.Id = this.Id;
            model.Amount = this.Amount;
            model.Status = this.Status;
            model.PaymentMethod = this.PaymentMethod;
            model.AccountId = this.AccountId;
            return model;
        }
    }
}
