using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Models;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportEngineTool
    {
        ReportEngineToolEnum ReportEngineTool { get; }
        Task<string> CreateGroup(string name);
        Task<ReportEngineToolReportModel> CreateReport(string groupId, string templateReportId, string name);
        Task DeleteReport(string groupId, string reportId);
        Task<ReportEngineToolReportModel> Deploy(Stream file, string groupId, string datasetName);
    }
}
