using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportEngineTool _reportEngineTool;

        public ReportService(IReportEngineTool reportEngineTool)
        {
            _reportEngineTool = reportEngineTool;
        }

        public async Task<ReportModel> CreateReport(string groupId, string templateReportId, string name)
        {
            return await _reportEngineTool.CreateReport(groupId, templateReportId, name);
        }

        public async Task DeleteReport(string groupId, string reportId)
        {
            await _reportEngineTool.DeleteReport(groupId, reportId);
        }

        public async Task<ReportModel> Deploy(Stream file, string groupId, string name)
        {
            return await _reportEngineTool.Deploy(file, groupId, name);
        }

    }
}
