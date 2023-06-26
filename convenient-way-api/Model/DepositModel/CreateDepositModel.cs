using ship_convenient.Constants.DepositConstant;
using ship_convenient.Entities;

namespace ship_convenient.Model.DepositModel
{
    public class CreateDepositModel
    {
        public int Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid AccountId { get; set; }

        public Deposit ToEntity()
        {
            Deposit entity = new Deposit();
            entity.Amount = this.Amount;
            entity.Status = DepositStatus.PENDING;
            entity.PaymentMethod = this.PaymentMethod;
            entity.AccountId = this.AccountId;
            return entity;
        }
    }
}
