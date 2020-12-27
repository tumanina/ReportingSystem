using System;

namespace ReportingSystem.Shared.Models
{
    public class ReportModel : BaseModel
    {
        public string Name { get; set; }

        public string DatasetId { get; set; }

        public Guid GroupId { get; set; }
        public GroupModel Group { get; set; }

        public Guid TemplateId { get; set; }
        public TemplateModel Template { get; set; }
    }
}
