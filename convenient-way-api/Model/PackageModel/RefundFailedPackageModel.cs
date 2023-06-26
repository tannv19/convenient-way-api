namespace ship_convenient.Model.PackageModel
{
    public class RefundFailedPackageModel
    {
        public Guid PackageId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
