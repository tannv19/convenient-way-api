namespace ship_convenient.Model.VnPayModel
{
    public class PaymentVnPayModel
    {
        public string Ip { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public long Amount { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
