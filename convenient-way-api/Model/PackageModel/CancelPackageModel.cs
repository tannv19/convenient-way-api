namespace ship_convenient.Model.PackageModel
{
    public class CancelPackageModel
    {
        public Guid PackageId { get; set; }
        public string? Reason { get; set; }
    }
}
