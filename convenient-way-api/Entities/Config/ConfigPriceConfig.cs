using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class ConfigPriceConfig : IEntityTypeConfiguration<ConfigPrice>
    {
        public void Configure(EntityTypeBuilder<ConfigPrice> builder)
        {
            builder.ToTable("ConfigPrice");
            builder.Property(x => x.ModifiedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();
        }
    }
}
