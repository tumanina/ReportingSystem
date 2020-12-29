using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportDalService _dalService;

        public ReportService(IReportDalService dalService)
        {
            _dalService = dalService;
        }

        public async Task<Guid> CreateReport(Guid groupId, string name)
        {
            return await _dalService.AddReport(new ReportModel { GroupId = groupId, Name = name });
        }

        public async Task DeleteReport(Guid reportId)
        {
            await _dalService.DeleteReport(reportId);
        }

        public async Task<ReportModel> GetReport(Guid reportId)
        {
            return await _dalService.GetReport(reportId);
        }
    }
}
