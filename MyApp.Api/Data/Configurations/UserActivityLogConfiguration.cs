using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidFlow.Data.Configurations
{
    public class UserActivityLogConfiguration : IEntityTypeConfiguration<UserActivityLog>
    {
        public void Configure(EntityTypeBuilder<UserActivityLog> builder)
        {
            
            builder.ToTable("UserActivityLogs");

            
            builder.HasKey(ual => ual.Id);

            
            builder.Property(ual => ual.UserId)
                .IsRequired();

            builder.Property(ual => ual.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ual => ual.Description)
                .HasMaxLength(500);

            builder.Property(ual => ual.IpAddress)
                .HasMaxLength(45); 

            builder.Property(ual => ual.UserAgent)
                .HasMaxLength(500);

            builder.Property(ual => ual.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            
            builder.HasIndex(ual => ual.UserId)
                .HasDatabaseName("IX_UserActivityLogs_UserId");

            builder.HasIndex(ual => ual.CreatedAt)
                .HasDatabaseName("IX_UserActivityLogs_CreatedAt");

            builder.HasIndex(ual => ual.Action)
                .HasDatabaseName("IX_UserActivityLogs_Action");

            
            builder.HasOne(ual => ual.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(ual => ual.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
