using ReportingSystem.Shared.Enums;
using System;

namespace ReportingSystem.Web.Models
{
    public class DeployReportApiModel
    {
        public Guid TemplateVersionId { get; set; }

        public ReportEngineToolEnum ReportEngineTool { get; set; }
    }
}
