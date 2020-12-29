using ReportingSystem.Shared.Enums;

namespace ReportingSystem.Shared.Models
{
    public class ReportEngineToolReportModel : BaseModel
    {
        public string ReportId { get; set; }

        public string DatasetId { get; set; }

        public ReportEngineToolEnum ReportEngineTool { get; set; }
    }
}
