using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services.Database
{
    public partial class TravelBayDbContext : DbContext
    {
        public TravelBayDbContext(DbContextOptions<TravelBayDbContext> options) : base(options)
        {
        }

        // Reference data
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Auth
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Asset> Assets { get; set; }

        // Domain
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<DestinationImage> DestinationImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TripPlan> TripPlans { get; set; }
        public DbSet<TripPlanItem> TripPlanItems { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<SavedDestination> SavedDestinations { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<ViewHistory> ViewHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CreateConfiguration(modelBuilder);

            CreateSeed(modelBuilder);

        }


    }
}
