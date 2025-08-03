using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserActivityLog>()
                .HasOne(log => log.PerformedByUser)
                .WithMany()
                .HasForeignKey(log => log.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserActivityLog>()
                .HasOne(log => log.TargetUser)
                .WithMany()
                .HasForeignKey(log => log.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
