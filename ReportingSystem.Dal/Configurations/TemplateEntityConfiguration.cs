using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;

namespace ReportingSystem.Dal.Configurations
{
    internal class TemplateEntityConfiguration : BaseConfiguration<TemplateEntity>
    {
        public new void Configure(EntityTypeBuilder<TemplateEntity> builder)
        {
            builder.ToTable("Templates");

            base.Configure(builder);

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();
        }
    }

}
