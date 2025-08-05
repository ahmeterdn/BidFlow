using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidFlow.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            
            builder.ToTable("Users");

            
            builder.HasKey(u => u.Id);

            
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.FirstName)
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .HasMaxLength(50);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            
            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.CreatedBy)
                .HasMaxLength(100);

            builder.Property(u => u.UpdatedBy)
                .HasMaxLength(100);

            
            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_Users_Username");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");

            
            builder.HasMany(u => u.ActivityLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
