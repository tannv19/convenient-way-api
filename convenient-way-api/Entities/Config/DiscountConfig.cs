using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class DiscountConfig : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discount");
            builder.HasMany(ds => ds.Packages)
                    .WithOne(pk => pk.Discount).HasForeignKey(pk => pk.DiscountId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
