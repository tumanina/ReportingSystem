using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.Configurations
{
    internal class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Password)
                   .HasMaxLength(100)
                   .IsRequired();
        }
    }

}
