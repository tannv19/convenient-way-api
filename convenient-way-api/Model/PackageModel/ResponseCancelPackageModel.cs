using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Model.PackageModel
{
    public class ResponseCancelPackageModel : ResponsePackageModel
    {
        public string? Reason { get; set; } = string.Empty;
        public DateTime CancelTime { get; set; }
    }
}
