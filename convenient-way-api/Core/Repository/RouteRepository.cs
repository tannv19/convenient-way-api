using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Core.Repository
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        public RouteRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
