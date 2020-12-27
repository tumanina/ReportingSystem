using ReportingSystem.Shared.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces
{
    public interface IReportService
    {
        Task<Guid> CreateReport(Guid groupId, string name);
        Task DeleteReport(Guid reportId);
    }
}
