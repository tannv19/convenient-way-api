namespace ship_convenient.Model.DashboardModel
{
    public class PackageCountModel
    {
        public int All { get; set; }
        public int Waiting { get; set; }
        public int Approved { get; set; }
        public int Reject { get; set; }
        public int Selected { get; set; }
        public int PickupSuccess { get; set; }
        public int PickupFailed { get; set; }
        public int DeliveredSuccess { get; set; }
        public int DeliveredFailed { get; set; }
        public int Success { get; set; }
        public int RefundToWarehouseSuccess { get; set; }
        public int RefundToWarehouseFailed { get; set; }

        public int DeliverCancel { get; set; }
        public int SenderCancel { get; set; }
        public int Expired { get; set; }
    }
}
