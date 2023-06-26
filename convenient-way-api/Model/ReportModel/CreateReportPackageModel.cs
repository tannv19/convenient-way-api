namespace ship_convenient.Model.ReportModel
{
    public class CreateReportPackageModel
    {
        public string Reason { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid PackageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
