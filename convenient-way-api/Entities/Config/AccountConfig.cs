using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            // builder.HasIndex(ac => ac.UserName).IsUnique();
            builder.Property(ac => ac.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.HasMany(ac => ac.Notifications)
                .WithOne(noti => noti.Account).HasForeignKey(noti => noti.AccountId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(ac => ac.PackageSenders)
                .WithOne(pa => pa.Sender).HasForeignKey(pa => pa.SenderId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.PackageDelivers)
                .WithOne(pa => pa.Deliver).HasForeignKey(pa => pa.DeliverId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.CreatorFeedbacks)
                .WithOne(f => f.Creator).HasForeignKey(f => f.CreatorId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.ReceiverFeedbacks)
                .WithOne(f => f.Receiver).HasForeignKey(f => f.ReceiverId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.CreatorReports)
                .WithOne(re => re.Creator).HasForeignKey(re => re.CreatorId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.ReceiverReports)
                .WithOne(re => re.Receiver).HasForeignKey(re => re.ReceiverId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(ac => ac.Transactions)
                .WithOne(tr => tr.Account).HasForeignKey(tr => tr.AccountId);
            builder.HasOne(ac => ac.InfoUser)
                .WithOne(info => info.Account).HasForeignKey<InfoUser>(info => info.AccountId);
        }
    }
}
