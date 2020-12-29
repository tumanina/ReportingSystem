using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportManager
    {
        Task<ReportModel> Deploy(ReportEngineToolEnum reportTool, Guid reportId, Guid templateVersionId);
        Task<ReportModel> CreateReport(ReportEngineToolEnum reportTool, string groupId, string templateReportId, string name);
        Task DeleteReport(Guid reportId);
    }
}
