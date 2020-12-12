using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.Configurations
{
    internal class ActionEntityConfiguration : IEntityTypeConfiguration<ActionEntity>
    {
        public void Configure(EntityTypeBuilder<ActionEntity> builder)
        {
            builder.ToTable("Actionss");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();
        }
    }

}
