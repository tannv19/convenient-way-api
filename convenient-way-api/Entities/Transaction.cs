using unitofwork_core.Model.TransactionModel;

namespace ship_convenient.Entities
{
    public class Transaction : BaseEntity
    {
        public string Status { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int CoinExchange { get; set; }
        public int BalanceWallet { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Relationship
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public Guid? DepositId { get; set; }
        public Deposit? Deposit { get; set; }
        public Guid? PackageId { get; set; }
        public Package? Package { get; set; }
        #endregion

        public ResponseTransactionModel ToResponseModel()
        {
            ResponseTransactionModel model = new ResponseTransactionModel();
            model.Title = this.Title;
            model.Status = this.Status;
            model.Description = this.Description;
            model.TransactionType = this.TransactionType;
            model.CoinExchange = this.CoinExchange;
            model.BalanceWallet = this.BalanceWallet;
            model.PackageId = this.PackageId;
            model.CreatedAt = this.CreatedAt;
            return model;
        }
    }
}
