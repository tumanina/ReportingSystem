using ReportingSystem.Shared.Models;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportService
    {
        Task<ReportModel> CreateReport(string groupId, string templateReportId, string name);
        Task DeleteReport(string groupId, string reportId);
        Task<ReportModel> Deploy(Stream file, string groupId, string name);
    }
}
