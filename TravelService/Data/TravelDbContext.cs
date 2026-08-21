using TravelService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace TravelService.Data
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options) { }

        public DbSet<Travel> Travels { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Destination>()
                .HasOne(d => d.Travel).WithMany(t => t.Destinations)
                .HasForeignKey(d => d.TravelId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Travel).WithMany(t => t.Activities)
                .HasForeignKey(a => a.TravelId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Travel).WithMany(t => t.Expenses)
                .HasForeignKey(e => e.TravelId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}