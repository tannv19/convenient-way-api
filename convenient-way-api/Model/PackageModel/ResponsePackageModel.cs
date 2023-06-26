using ship_convenient.Model.TransactionPackageModel;
using ship_convenient.Model.UserModel;
using System.ComponentModel.DataAnnotations;
using unitofwork_core.Model.ProductModel;

namespace unitofwork_core.Model.PackageModel
{
    public class ResponsePackageModel
    {
        public Guid Id { get; set; }
        public string StartAddress { get; set; } = string.Empty;
        public double StartLongitude { get; set; }
        public double StartLatitude { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public string PickupName { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        [Phone]
        public string ReceiverPhone { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? PriceShip { get; set; } = null;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime PickupTimeStart { get; set; }
        public DateTime PickupTimeOver { get; set; }
        public DateTime DeliveryTimeStart { get; set; }
        public DateTime DeliveryTimeOver { get; set; }
        public DateTime ExpiredTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Guid SenderId { get; set; }   
        public ResponseAccountModel? Sender { get; set; }
        public Guid? DeliverId { get; set; }
        public ResponseAccountModel? Deliver { get; set; }
        public List<ResponseProductModel> Products { get; set; } = new List<ResponseProductModel>();
        public List<ResponseTransactionPackageModel> PackageTransactions { get; set; } = new List<ResponseTransactionPackageModel>();


        public int GetPriceProducts()
        {
            int price = 0;
            foreach (var product in Products)
            {
                price += product.Price;
            }
            return price;
        }
    }
}
