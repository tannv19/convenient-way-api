using ship_convenient.Model.UserModel;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Model.ReportModel
{
    public class ResponseReportModel
    {
        public Guid Id { get; set; }
        public string TypeOfReport { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Guid PackageId { get; set; }
        public ResponsePackageModel? Package { get; set; }
        public Guid CreatorId { get; set; }
        public ResponseAccountModel? Creator { get; set; }
        public Guid ReceiverId { get; set; }
        public ResponseAccountModel? Receiver { get; set; }


    }
}
