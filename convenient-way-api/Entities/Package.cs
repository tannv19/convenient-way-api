using ship_convenient.Model.PackageModel;
using unitofwork_core.Constant.Package;
using unitofwork_core.Model.PackageModel;

namespace ship_convenient.Entities
{
    public class Package : BaseEntity
    {
        public string StartAddress { get; set; } = string.Empty;
        public double StartLongitude { get; set; }
        public double StartLatitude { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double Distance { get; set; }
        public string PickupName { get; set; } = string.Empty;
        public string PickupPhone { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PriceShip { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public DateTime PickupTimeStart { get; set; }
        public DateTime PickupTimeOver { get; set; }
        public DateTime DeliveryTimeStart { get; set; }
        public DateTime DeliveryTimeOver { get; set; }
        public DateTime ExpiredTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        #region Relationship
        public Guid SenderId { get; set; }
        public Account? Sender { get; set; }
        public Guid? DeliverId { get; set; }
        public Account? Deliver { get; set; }
        public Guid? DiscountId { get; set; }
        public Discount? Discount { get; set; }

        public List<Transaction> Transactions { get; set; }
        public List<TransactionPackage> TransactionPackages { get; set; }
        public List<Product> Products { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public List<Report> Reports { get; set; }
        public List<Notification> Notifications { get; set; }
        #endregion

        public Package()
        {
            Transactions = new List<Transaction>();
            TransactionPackages = new List<TransactionPackage>();
            Products = new List<Product>();
            Feedbacks = new List<Feedback>();
            Reports = new List<Report>();
            Notifications = new List<Notification>();
        }

        public ResponsePackageModel ToResponseModel()
        {
            ResponsePackageModel model = new ResponsePackageModel();
            model.Id = this.Id;
            model.StartAddress = this.StartAddress;
            model.StartLongitude = this.StartLongitude;
            model.StartLatitude = this.StartLatitude;
            model.DestinationAddress = this.DestinationAddress;
            model.DestinationLongitude = this.DestinationLongitude;
            model.DestinationLatitude = this.DestinationLatitude;
            model.PickupName = this.PickupName;
            model.PickupPhone = this.PickupPhone;
            model.ReceiverName = this.ReceiverName;
            model.ReceiverPhone = this.ReceiverPhone;
            model.Distance = this.Distance;
            model.Height = this.Height;
            model.Width = this.Width;
            model.Length = this.Length;
            model.Weight = this.Weight;
            model.Status = this.Status;
            model.PriceShip = this.PriceShip;
            model.PhotoUrl = this.PhotoUrl;
            model.Note = this.Note;
            model.PickupTimeStart = this.PickupTimeStart;
            model.PickupTimeOver = this.PickupTimeOver;
            model.DeliveryTimeStart = this.DeliveryTimeStart;
            model.DeliveryTimeOver = this.DeliveryTimeOver;
            model.ExpiredTime = this.ExpiredTime;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            model.SenderId = this.SenderId;
            model.DeliverId = this.DeliverId;
            model.Sender = this.Sender != null ? this.Sender.ToResponseModel() : null;
            model.Deliver = this.Deliver != null ? this.Deliver.ToResponseModel() : null;

            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                model.Products.Add(this.Products[i].ToResponseModel());
            }

            int countTransaction = this.TransactionPackages.Count;
            for (int i = 0; i < countTransaction; i++)
            {
                model.PackageTransactions.Add(this.TransactionPackages[i].ToResponseModel());
            }
            return model;
        }

        public ResponseSuggestPackageModel ToResponseSuggestModel()
        {
            ResponseSuggestPackageModel model = new ResponseSuggestPackageModel();
            model.Id = this.Id;
            model.StartAddress = this.StartAddress;
            model.StartLongitude = this.StartLongitude;
            model.StartLatitude = this.StartLatitude;
            model.DestinationAddress = this.DestinationAddress;
            model.DestinationLongitude = this.DestinationLongitude;
            model.DestinationLatitude = this.DestinationLatitude;
            model.PickupName = this.PickupName;
            model.PickupPhone = this.PickupPhone;
            model.ReceiverName = this.ReceiverName;
            model.ReceiverPhone = this.ReceiverPhone;
            model.Distance = this.Distance;
            model.Height = this.Height;
            model.Width = this.Width;
            model.Length = this.Length;
            model.Weight = this.Weight;
            model.Status = this.Status;
            model.PriceShip = this.PriceShip;
            model.PhotoUrl = this.PhotoUrl;
            model.Note = this.Note;
            model.PickupTimeStart = this.PickupTimeStart;
            model.PickupTimeOver = this.PickupTimeOver;
            model.DeliveryTimeStart = this.DeliveryTimeStart;
            model.DeliveryTimeOver = this.DeliveryTimeOver;
            model.ExpiredTime = this.ExpiredTime;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            model.SenderId = this.SenderId;
            model.DeliverId = this.DeliverId;
            model.Sender = this.Sender != null ? this.Sender.ToResponseModel() : null;
            model.Deliver = this.Deliver != null ? this.Deliver.ToResponseModel() : null;

            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                model.Products.Add(this.Products[i].ToResponseModel());
            }

            int countTransaction = this.TransactionPackages.Count;
            for (int i = 0; i < countTransaction; i++)
            {
                model.PackageTransactions.Add(this.TransactionPackages[i].ToResponseModel());
            }
            return model;
        }
        public ResponseCancelPackageModel ToDeliverCancelPackage()
        {
            ResponseCancelPackageModel model = new ResponseCancelPackageModel();
            model.Id = this.Id;
            model.StartAddress = this.StartAddress;
            model.StartLongitude = this.StartLongitude;
            model.StartLatitude = this.StartLatitude;
            model.DestinationAddress = this.DestinationAddress;
            model.DestinationLongitude = this.DestinationLongitude;
            model.DestinationLatitude = this.DestinationLatitude;
            model.PickupName = this.PickupName;
            model.PickupPhone = this.PickupPhone;
            model.ReceiverName = this.ReceiverName;
            model.ReceiverPhone = this.ReceiverPhone;
            model.Distance = this.Distance;
            model.Length = this.Length;
            model.Width = this.Width;
            model.Height = this.Height;
            model.Weight = this.Weight;
            model.Status = this.Status;
            model.PriceShip = this.PriceShip;
            model.PhotoUrl = this.PhotoUrl;
            model.Note = this.Note;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            model.SenderId = this.SenderId;
            model.DeliverId = this.DeliverId;
            model.Sender = this.Sender != null ? this.Sender.ToResponseModel() : null;
            model.Deliver = this.Deliver != null ? this.Deliver.ToResponseModel() : null;

            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                model.Products.Add(this.Products[i].ToResponseModel());
            }
            TransactionPackage? cancelTrans = this.TransactionPackages.SingleOrDefault(trans => trans.ToStatus == PackageStatus.DELIVER_CANCEL);
            if (cancelTrans != null)
            {
                model.Reason = cancelTrans.Reason;
                model.CancelTime = cancelTrans.CreatedAt;
            }
            return model;
        }

        public ResponseCancelPackageModel ToSenderCancelPackage()
        {
            ResponseCancelPackageModel model = new ResponseCancelPackageModel();
            model.Id = this.Id;
            model.StartAddress = this.StartAddress;
            model.StartLongitude = this.StartLongitude;
            model.StartLatitude = this.StartLatitude;
            model.DestinationAddress = this.DestinationAddress;
            model.DestinationLongitude = this.DestinationLongitude;
            model.DestinationLatitude = this.DestinationLatitude;
            model.PickupName = this.PickupName;
            model.PickupPhone = this.PickupPhone;
            model.ReceiverName = this.ReceiverName;
            model.ReceiverPhone = this.ReceiverPhone;
            model.Distance = this.Distance;
            model.Length = this.Length;
            model.Width = this.Width;
            model.Height = this.Height;
            model.Weight = this.Weight;
            model.Status = this.Status;
            model.PriceShip = this.PriceShip;
            model.PhotoUrl = this.PhotoUrl;
            model.Note = this.Note;
            model.CreatedAt = this.CreatedAt;
            model.ModifiedAt = this.ModifiedAt;
            model.SenderId = this.SenderId;
            model.DeliverId = this.DeliverId;
            model.Sender = this.Sender != null ? this.Sender.ToResponseModel() : null;
            model.Deliver = this.Deliver != null ? this.Deliver.ToResponseModel() : null;

            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                model.Products.Add(this.Products[i].ToResponseModel());
            }
            TransactionPackage? cancelTrans = this.TransactionPackages.SingleOrDefault(trans => trans.ToStatus == PackageStatus.SENDER_CANCEL);
            if (cancelTrans != null)
            {
                model.Reason = cancelTrans.Reason;
                model.CancelTime = cancelTrans.CreatedAt;
            }
            return model;
        }

        public int GetPricePackage() {
            int price = 0;
            int countProduct = this.Products.Count;
            for (int i = 0; i < countProduct; i++)
            {
                price += this.Products[i].Price;
            }
            return price;
        }
    }
}



