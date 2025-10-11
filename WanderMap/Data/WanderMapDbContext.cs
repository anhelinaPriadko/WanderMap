using Microsoft.EntityFrameworkCore;
using WanderMap.Models;

namespace WanderMap.Data
{
    public class WanderMapDbContext : DbContext
    {
        public WanderMapDbContext(DbContextOptions<WanderMapDbContext> options)
            : base(options)
        {
        }

        public DbSet<Place> Places { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Photo> Photos { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------
            // Category
            // ---------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Slug).IsRequired().HasMaxLength(120);
                entity.Property(c => c.Description).HasMaxLength(500).IsRequired(false);
            });

            // ---------------------
            // Event
            // ---------------------
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(120).IsRequired();
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Events)
                      .IsRequired(false)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Place)
                      .WithMany(p => p.Events)
                      .HasForeignKey(e => e.PlaceId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Event_Place");
                entity.Property(e => e.StartAt).IsRequired();
                entity.Property(e => e.EndAt).IsRequired(false);
                entity.Property(e => e.WebsiteUrl).HasMaxLength(200)
                      .IsRequired(false);
                entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)")
                      .IsRequired(false);
                entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)")
                      .IsRequired(false);
            });

            // ---------------------
            // Place
            // ---------------------
            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Slug).HasMaxLength(120).IsRequired();
                entity.Property(c => c.Description).HasMaxLength(500).IsRequired(false);
                entity.Property(p => p.Address).HasMaxLength(200).IsRequired(false);
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Places)
                      .HasForeignKey(p => p.CategoryId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.MainPhoto)
                      .WithMany() // no inverse navigation for "main"
                      .HasForeignKey(p => p.MainPhotoId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(p => p.Latitude).HasColumnType("decimal(18,12)").IsRequired(false);
                entity.Property(p => p.Longitude).HasColumnType("decimal(18,12)").IsRequired(false);
                entity.Property(p => p.WebsiteUrl).HasMaxLength(200).IsRequired(false);
                entity.Property(p => p.ContactPhone).HasMaxLength(20).IsRequired(false);
                entity.Property(p => p.IsDeleted).HasDefaultValue(false).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired(false);
                entity.Property(p => p.CreatedAt).IsRequired();
            });

            // ---------------------
            // Photo
            // ---------------------
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(ph => ph.Id);
                entity.Property(ph => ph.Url).IsRequired();
                entity.Property(ph => ph.OriginalFileName).HasMaxLength(255);
                entity.Property(ph => ph.ContentType).HasMaxLength(100);
                entity.Property(ph => ph.Size).IsRequired();

                entity.HasOne(ph => ph.Event)
                      .WithMany(e => e.Photos)
                      .HasForeignKey(ph => ph.EventId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ph => ph.UploadedBy)
                      .WithMany(ph => ph.Photos)
                      .HasForeignKey(ph => ph.UploadedById)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(ph => ph.Place)
                        .WithMany(p => p.Photos)
                        .HasForeignKey(ph => ph.PlaceId)
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.Property(ph => ph.IsMain).HasDefaultValue(false).IsRequired();
                entity.Property(ph => ph.SortOrder).HasDefaultValue(0).IsRequired();
                entity.Property(ph => ph.CreatedAt).IsRequired();
            });

            // ---------------------
            // Review
            // ---------------------
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Comment).HasMaxLength(500).IsRequired(false);

                entity.HasOne(r => r.Place)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(r => r.PlaceId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Event)
                      .WithMany(e => e.Reviews)
                      .HasForeignKey(r => r.EventId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.UserId).IsRequired();
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews) 
                      .HasForeignKey(r => r.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.CreatedAt).IsRequired();
            });

            // ---------------------
            // User
            // ---------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
                entity.Property(u => u.UserName).HasMaxLength(100).IsRequired(false);
                entity.Property(u => u.PasswordHash).IsRequired(false);
                entity.Property(u => u.AvatarUrl).IsRequired(false);
                entity.Property(u => u.ExternalProvider).HasMaxLength(100).IsRequired(false);
                entity.Property(u => u.ExternalId).HasMaxLength(200).IsRequired(false);
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.LastLoginAt).IsRequired(false);
            });
        }
    }
}
