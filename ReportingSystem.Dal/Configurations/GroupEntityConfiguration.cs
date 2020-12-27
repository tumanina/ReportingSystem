using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.Configurations
{
    internal class GroupEntityConfiguration : BaseConfiguration<GroupEntity>
    {
        public new void Configure(EntityTypeBuilder<GroupEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Groups");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();
        }
    }

}
