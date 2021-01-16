﻿using Microsoft.EntityFrameworkCore;
using ReportingSystem.Dal.Configurations;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.DbContexts
{
    public class ReportingDbContext : DbContext
    {
        public ReportingDbContext(DbContextOptions<ReportingDbContext> dbOptions) : base(dbOptions)
        { }

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<AccountActionEntity> AccountActions { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<ReportEntity> Reports { get; set; }
        public DbSet<TemplateEntity> Templates { get; set; }
        public DbSet<TemplateVersionEntity> TemplateVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ActionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AccountEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AccountActionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ReportEntityConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateVersionEntityConfiguration());
        }
    }
}
