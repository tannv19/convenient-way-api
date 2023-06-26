using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class RouteConfig : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable("Route");
            builder.HasMany(route => route.RoutePoints)
               .WithOne(routePoint => routePoint.Route).HasForeignKey(routePoint => routePoint.RouteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
