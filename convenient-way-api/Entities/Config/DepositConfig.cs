using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class DepositConfig : IEntityTypeConfiguration<Deposit>
    {
        public void Configure(EntityTypeBuilder<Deposit> builder)
        {
            builder.ToTable("Deposit");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.PaymentMethod).IsRequired();
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.Property(x => x.ModifiedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();
            builder.Property(x => x.AccountId).IsRequired();
            builder.HasOne(x => x.Account).WithMany(x => x.Deposits).HasForeignKey(x => x.AccountId);
            builder.HasMany(x => x.Transactions).WithOne(x => x.Deposit).HasForeignKey(x => x.DepositId);
        }
    }
    
}
