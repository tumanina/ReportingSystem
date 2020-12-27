using System;

namespace ReportingSystem.Dal.Entities
{
    public class TemplateVersionEntity : BaseEntity
    {
        public string FileName { get; set; }
        public string Version { get; set; }

        public Guid TemplateId { get; set; }

        public virtual TemplateEntity Template { get; set; }
    }
}
