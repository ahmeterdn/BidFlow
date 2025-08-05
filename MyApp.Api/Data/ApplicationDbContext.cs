using BidFlow.Data;
using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, AuditInterceptor auditInterceptor)
            : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            ConfigurePostgreSqlConventions(modelBuilder);
        }

        private static void ConfigurePostgreSqlConventions(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        property.SetColumnName(ToSnakeCase(columnName));
                    }
                }

                foreach (var index in entity.GetIndexes())
                {
                    var indexName = index.GetDatabaseName();
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        index.SetDatabaseName(ToSnakeCase(indexName));
                    }
                }
            }
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var result = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                    result += "_";
                result += char.ToLower(input[i]);
            }
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
