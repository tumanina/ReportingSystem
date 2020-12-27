using System;

namespace ReportingSystem.Dal.Entities
{
    public class ReportEntity : BaseEntity
    {
        public string Name { get; set; }

        public Guid GroupId { get; set; }

        public virtual GroupEntity Group { get; set; }

        public Guid TemplateId { get; set; }

        public virtual TemplateEntity Template { get; set; }
    }
}
