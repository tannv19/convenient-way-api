using ship_convenient.Core.Context;
using ship_convenient.Core.IRepository;
using ship_convenient.Entities;

namespace ship_convenient.Core.Repository
{
    public class InfoUserRepository : GenericRepository<InfoUser>, IInfoUserRepository
    {
        public InfoUserRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
