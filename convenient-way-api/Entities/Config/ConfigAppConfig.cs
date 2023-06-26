using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class ConfigAppConfig : IEntityTypeConfiguration<ConfigApp>
    {
        public void Configure(EntityTypeBuilder<ConfigApp> builder)
        {
            builder.ToTable("Config");
            builder.Property(x => x.ModifiedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();

        }
    }
}
