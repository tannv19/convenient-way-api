using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class FeedbackConfig : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");
            builder.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
            builder.Property(f => f.ModifiedAt)
                .HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAddOrUpdate();
            builder.HasOne(f => f.Package).WithMany(p => p.Feedbacks).HasForeignKey(f => f.PackageId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
