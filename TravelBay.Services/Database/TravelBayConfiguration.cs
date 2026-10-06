using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services.Database
{
    public partial class TravelBayDbContext : DbContext
    {

        private void CreateConfiguration(ModelBuilder modelBuilder)
        {
            // ----- Reference data -----
            modelBuilder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(co => co.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Auth -----
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfileImage)
                .WithMany()
                .HasForeignKey(u => u.ProfileImageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ----- Destination -----
            modelBuilder.Entity<Destination>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Destinations)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Destination>()
                .HasOne(d => d.City)
                .WithMany(c => c.Destinations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Destination>()
                .HasQueryFilter(d => !d.IsDeleted);

            modelBuilder.Entity<DestinationImage>()
                .HasOne(di => di.Destination)
                .WithMany(d => d.Images)
                .HasForeignKey(di => di.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DestinationImage>()
                .HasOne(di => di.Asset)
                .WithMany()
                .HasForeignKey(di => di.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<News>()
                .HasOne(n => n.ImageAsset)
                .WithMany()
                .HasForeignKey(n => n.ImageAssetId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Review -----
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Destination)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ModeratedByUser)
                .WithMany()
                .HasForeignKey(r => r.ModeratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasQueryFilter(r => !r.IsDeleted);

            // ----- TripPlan -----
            modelBuilder.Entity<TripPlan>()
                .HasOne(tp => tp.User)
                .WithMany()
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripPlan>()
                .HasQueryFilter(tp => !tp.IsDeleted);

            modelBuilder.Entity<TripPlanItem>()
                .HasOne(tpi => tpi.TripPlan)
                .WithMany(tp => tp.Items)
                .HasForeignKey(tpi => tpi.TripPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TripPlanItem>()
                .HasOne(tpi => tpi.Destination)
                .WithMany(d => d.TripPlanItems)
                .HasForeignKey(tpi => tpi.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Collection -----
            modelBuilder.Entity<Collection>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.Collection)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.Destination)
                .WithMany(d => d.CollectionItems)
                .HasForeignKey(ci => ci.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- SavedDestination -----
            modelBuilder.Entity<SavedDestination>()
                .HasOne(sd => sd.User)
                .WithMany()
                .HasForeignKey(sd => sd.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedDestination>()
                .HasOne(sd => sd.Destination)
                .WithMany(d => d.SavedByUsers)
                .HasForeignKey(sd => sd.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedDestination>()
                .HasIndex(sd => new { sd.UserId, sd.DestinationId })
                .IsUnique();

            // ----- UserPreference -----
            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Category)
                .WithMany(c => c.UserPreferences)
                .HasForeignKey(up => up.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPreference>()
                .HasIndex(up => new { up.UserId, up.CategoryId })
                .IsUnique();

            // ----- ViewHistory -----
            modelBuilder.Entity<ViewHistory>()
                .HasOne(vh => vh.User)
                .WithMany()
                .HasForeignKey(vh => vh.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViewHistory>()
                .HasOne(vh => vh.Destination)
                .WithMany(d => d.ViewHistoryEntries)
                .HasForeignKey(vh => vh.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Notification -----
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- AuditLog -----
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.PerformedByUser)
                .WithMany()
                .HasForeignKey(a => a.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
