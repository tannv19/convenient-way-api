namespace ship_convenient.Model.DepositModel
{
    public class ResponseDepositModel
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid AccountId;
    }
}
