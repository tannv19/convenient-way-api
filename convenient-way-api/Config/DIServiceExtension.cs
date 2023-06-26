using ship_convenient.Core.Context;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.AuthorizeService;
using ship_convenient.Services.ConfigService;
using ship_convenient.Services.ConfigUserService;
using ship_convenient.Services.DashboardService;
using ship_convenient.Services.DatabaseService;
using ship_convenient.Services.DepositService;
using ship_convenient.Services.FeedbackService;
using ship_convenient.Services.FirebaseCloudMsgService;
using ship_convenient.Services.GoongService;
using ship_convenient.Services.MapboxService;
using ship_convenient.Services.Notificationservice;
using ship_convenient.Services.PackageService;
using ship_convenient.Services.ReportService;
using ship_convenient.Services.RouteService;
using ship_convenient.Services.ScriptService;
using ship_convenient.Services.SendSmsService;
using ship_convenient.Services.TransactionPackageService;
using ship_convenient.Services.TransactionService;
using ship_convenient.Services.VnPayService;

namespace ship_convenient.Config
{
    public static class DIServiceExtension
    {
        public static void AddDIService(this IServiceCollection services) {
            services.AddDbContext<AppDbContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGoongService, GoongService>();
            services.AddTransient<IMapboxService, MapboxService>();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAuthorizeService, AuthorizeService>();
            services.AddTransient<IRouteService, RouteService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<ITransactionPackageService, TransactionPackageService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddTransient<IDepositService, DepositService>();
            services.AddTransient <IFeedbackService,FeedbackService>();
            services.AddTransient<IFirebaseCloudMsgService, FirebaseCloudMsgService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<IConfigUserService, ConfigUserService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<ISendSMSService, SendSmsService>();
            services.AddTransient<IScriptService, ScriptService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<PackageUtils>();
            services.AddScoped<AccountUtils>();  
            services.AddScoped<PackageSuggestService>();
            services.AddScoped<IPackageServiceScript, PackageServiceScript>();
        }
    }
}
