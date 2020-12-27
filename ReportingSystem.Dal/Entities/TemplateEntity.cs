using System;
using System.Collections.Generic;

namespace ReportingSystem.Dal.Entities
{
    public class TemplateEntity : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<ReportEntity> Reports { get; set; }

        public virtual ICollection<TemplateVersionEntity> Versions { get; set; }
    }
}
