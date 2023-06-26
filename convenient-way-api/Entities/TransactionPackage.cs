using ship_convenient.Model.TransactionPackageModel;

namespace ship_convenient.Entities
{
    public class TransactionPackage : BaseEntity
    {
        public string FromStatus { get; set; } = string.Empty;
        public string ToStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Relationship
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        #endregion

        public ResponseTransactionPackageModel ToResponseModel()
        {
            ResponseTransactionPackageModel model = new ResponseTransactionPackageModel();
            model.FromStatus = this.FromStatus;
            model.ToStatus = this.ToStatus;
            model.Description = this.Description;
            model.Reason = this.Reason;
            model.ImageUrl = this.ImageUrl;
            model.PackageId = this.PackageId;
            model.CreatedAt = this.CreatedAt;
            return model;
        }
    }

 
}
