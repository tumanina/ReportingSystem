using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;
using System;

namespace ReportingSystem.Dal.Configurations
{
    internal class ReportEntityConfigurationConfiguration : BaseConfiguration<ReportEntity>
    {
        public new void Configure(EntityTypeBuilder<ReportEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Reports");

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasOne(x => x.Template)
                .WithMany(t => t.Reports)
                .HasForeignKey("TemplateId")
                .IsRequired();

            builder.HasOne(x => x.Group)
                .WithMany(t => t.Reports)
                .HasForeignKey("GroupId")
                .IsRequired();
        }
    }

}
