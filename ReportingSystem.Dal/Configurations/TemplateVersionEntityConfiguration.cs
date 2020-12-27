using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportingSystem.Dal.Entities;
using System;

namespace ReportingSystem.Dal.Configurations
{
    internal class TemplateVersionEntityConfiguration : BaseConfiguration<TemplateVersionEntity>
    {
        public new void Configure(EntityTypeBuilder<TemplateVersionEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("TemplateVersions");

            builder.Property(x => x.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(x => x.Version)
               .HasMaxLength(50)
               .IsRequired();

            builder.HasOne(x => x.Template)
                .WithMany(t => t.Versions)
                .HasForeignKey("TemplateId")
                .IsRequired();
        }
    }

}
