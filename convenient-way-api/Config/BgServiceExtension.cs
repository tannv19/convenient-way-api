using ship_convenient.BgService;

namespace ship_convenient.Config
{
    public static class BgServiceExtension
    {
        public static void AddBackgroundServices(this IServiceCollection services)
        {
            // services.AddHostedService<BgServiceNotifyTimePickup>();
            services.AddHostedService<BgServiceSuccessPackage>();
            services.AddHostedService<BgServiceExpiredPackage>();
            services.AddHostedService<BgServiceNotifySuggest>();
        }
    }
}
