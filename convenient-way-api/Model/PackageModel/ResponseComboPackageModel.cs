using ship_convenient.Model.UserModel;

namespace unitofwork_core.Model.PackageModel
{
    public class ResponseComboPackageModel
    {
        public ResponseAccountModel? Sender { get; set; }
        public double Time { get; set; }
        public double Distance { get; set; }
        public int ComboPrice { get; set; }
        public List<ResponsePackageModel> Packages = new List<ResponsePackageModel>();
    }
}
