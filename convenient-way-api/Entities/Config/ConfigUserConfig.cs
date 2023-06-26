using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class ConfigUserConfig : IEntityTypeConfiguration<ConfigUser>
    {
        public void Configure(EntityTypeBuilder<ConfigUser> builder)
        {
            builder.ToTable("ConfigUser");
            builder.Property(x => x.ModifiedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();
        }
    }
}
