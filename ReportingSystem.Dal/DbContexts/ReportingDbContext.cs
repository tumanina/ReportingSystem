using Microsoft.EntityFrameworkCore;
using ReportingSystem.Dal.Configurations;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.DbContexts
{
    public class ReportingDbContext : DbContext
    {
        public ReportingDbContext(DbContextOptions<ReportingDbContext> dbOptions) : base(dbOptions)
        { }

        public DbSet<AccountEntity> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ActionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AccountEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AccountActionEntityConfiguration());
        }
    }

    /*internal class CoreDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReportingDbContext>
    {
        public ReportingDbContext CreateDbContext(string[] args)
        {
            return new ReportingDbContext(
                new DbContextOptionsBuilder<ReportingDbContext>()
                .UseSqlServer(SettingsHelper.GetCoreSettings()?.DbConnectionString)
                .Options);
        }
    }*/
}
