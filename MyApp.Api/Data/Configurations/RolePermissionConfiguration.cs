using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidFlow.Data.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Table name
            builder.ToTable("RolePermissions");

            // Primary key
            builder.HasKey(rp => rp.Id);

            // Properties
            builder.Property(rp => rp.RoleId)
                .IsRequired();

            builder.Property(rp => rp.PermissionId)
                .IsRequired();

            builder.Property(rp => rp.IsActive)
                .HasDefaultValue(true);

            // Audit fields
            builder.Property(rp => rp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(rp => rp.CreatedBy)
                .HasMaxLength(100);

            builder.Property(rp => rp.UpdatedBy)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique()
                .HasDatabaseName("IX_RolePermissions_RoleId_PermissionId");

            builder.HasIndex(rp => rp.RoleId)
                .HasDatabaseName("IX_RolePermissions_RoleId");

            builder.HasIndex(rp => rp.PermissionId)
                .HasDatabaseName("IX_RolePermissions_PermissionId");

            builder.HasIndex(rp => rp.IsActive)
                .HasDatabaseName("IX_RolePermissions_IsActive");

            // Relationships
            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
