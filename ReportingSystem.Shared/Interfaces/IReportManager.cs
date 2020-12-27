using ReportingSystem.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportManager
    {
        Task<ReportModel> Deploy(string fileName, string name, string groupId);
        Task<ReportModel> CreateReport(string groupId, string templateReportId, string name);
        Task DeleteReport(Guid reportId);
    }
}
