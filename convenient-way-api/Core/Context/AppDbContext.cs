using Microsoft.EntityFrameworkCore;
using ship_convenient.Entities;
using System.Reflection;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Core.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        
        #region Dbset
        public virtual DbSet<Account> Accounts => Set<Account>();
        public virtual DbSet<InfoUser> InfoUsers => Set<InfoUser>();
        public virtual DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public virtual DbSet<Route> Routes => Set<Route>();
        public virtual DbSet<ConfigApp> Configs => Set<ConfigApp>();
        public virtual DbSet<Discount> Discounts => Set<Discount>();
        public virtual DbSet<Notification> Notifications => Set<Notification>();
        public virtual DbSet<Package> Packages => Set<Package>();
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Deposit> Deposits => Set<Deposit>();
        public virtual DbSet<Feedback> Feedbacks => Set<Feedback>();
        public virtual DbSet<Transaction> Transactions => Set<Transaction>();
        public virtual DbSet<TransactionPackage> TransactionPackages => Set<TransactionPackage>();
        public virtual DbSet<Report> Reports => Set<Report>();
        public virtual DbSet<ConfigUser> ConfigUsers => Set<ConfigUser>();
        public virtual DbSet<RoutePoint> RoutePoints => Set<RoutePoint>();
        public virtual DbSet<ConfigPrice> ConfigPrices => Set<ConfigPrice>();

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = _configuration.GetConnectionString("AzureConnection");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {

                connectionString = _configuration.GetConnectionString("DockerConnection");
     
            }
            else
            {
                connectionString = _configuration.GetConnectionString("AzureConnection");
            }
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var connectionString2 = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
            // if (!string.IsNullOrEmpty(connectionString)) optionsBuilder.UseSqlServer(connectionString);
            if (!string.IsNullOrEmpty(connectionString2)) optionsBuilder.UseSqlServer(connectionString2);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
