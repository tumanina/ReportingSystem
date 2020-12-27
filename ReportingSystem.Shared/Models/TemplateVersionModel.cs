using System;

namespace ReportingSystem.Shared.Models
{
    public class TemplateVersionModel : BaseModel
    {
        public string FileName { get; set; }
        public string Version { get; set; }

        public Guid TemplateId { get; set; }

        public virtual TemplateModel Template { get; set; }
    }
}
