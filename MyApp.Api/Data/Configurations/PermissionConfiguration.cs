using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidFlow.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Table name
            builder.ToTable("Permissions");

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.Property(p => p.Resource)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.EndpointPattern)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            // Audit fields
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.CreatedBy)
                .HasMaxLength(100);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("IX_Permissions_Name");

            builder.HasIndex(p => new { p.Resource, p.Action })
                .HasDatabaseName("IX_Permissions_Resource_Action");

            builder.HasIndex(p => p.EndpointPattern)
                .HasDatabaseName("IX_Permissions_EndpointPattern");

            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Permissions_IsActive");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Permissions_CreatedAt");

            // Relationships
            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
