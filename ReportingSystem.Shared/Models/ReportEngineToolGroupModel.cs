
using ReportingSystem.Shared.Enums;

namespace ReportingSystem.Shared.Models
{
    public class ReportEngineToolGroupModel : BaseModel
    {
        public string GroupId { get; set; }

        public ReportEngineToolEnum ReportEngineTool { get; set; }
    }
}
