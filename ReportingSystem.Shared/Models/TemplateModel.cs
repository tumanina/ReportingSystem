using System.Collections.Generic;

namespace ReportingSystem.Shared.Models
{
    public class TemplateModel : BaseModel
    {
        public string Name { get; set; }

        public virtual IEnumerable<ReportModel> Reports { get; set; }

        public virtual IEnumerable<TemplateVersionModel> Versions { get; set; }
    }
}
