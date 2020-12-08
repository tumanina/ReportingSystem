using ReportingSystem.Shared.Models;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportManager
    {
        Task DeleteReport(string groupId, string reportId);
        Task<ReportModel> Deploy(string fileName, string groupId, string name);
    }
}
