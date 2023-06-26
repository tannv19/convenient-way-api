namespace unitofwork_core.Model.TransactionModel
{
    public class ResponseTransactionModel
    {
        public string Status { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int CoinExchange { get; set; }
        public int BalanceWallet { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? PackageId { get; set; } = null;
    }
}
