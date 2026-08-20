using BackendAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Travel> Travels { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Destination configurations
            modelBuilder.Entity<Destination>()
                .HasOne(d => d.Travel)
                .WithMany(t => t.Destinations)
                .HasForeignKey(d => d.TravelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Activity configurations
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Travel)
                .WithMany(t => t.Activities)
                .HasForeignKey(a => a.TravelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Expense configurations
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Travel)
                .WithMany(t => t.Expenses)
                .HasForeignKey(e => e.TravelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}