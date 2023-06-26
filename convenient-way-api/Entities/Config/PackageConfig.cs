using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class PackageConfig : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Package");
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.Property(u => u.ModifiedAt)
                .HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();
            builder.HasMany(pa => pa.Products)
                .WithOne(pro => pro.Package).HasForeignKey(pro => pro.PackageId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pa => pa.Transactions)
                .WithOne(tr => tr.Package).HasForeignKey(tr => tr.PackageId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pa => pa.TransactionPackages)
                .WithOne(tr => tr.Package).HasForeignKey(tr => tr.PackageId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pa => pa.Reports)
                .WithOne(rp => rp.Package).HasForeignKey(rp => rp.PackageId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pa => pa.Feedbacks)
                .WithOne(fb => fb.Package).HasForeignKey(fb => fb.PackageId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pa => pa.Notifications)
                .WithOne(nt => nt.Package).HasForeignKey(nt => nt.PackageId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}
