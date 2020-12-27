using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Services
{
    public class ReportManager : IReportManager
    {
        private readonly IReportService _reportService;
        private readonly IFileService _fileService;
        private readonly IReportEngineTool _reportEngineTool;

        public ReportManager(IReportService reportService, IFileService fileService, IReportEngineTool reportEngineTool)
        {
            _reportService = reportService;
            _fileService = fileService;
            _reportEngineTool = reportEngineTool;
        }

        public async Task<ReportModel> CreateReport(string groupId, string templateReportId, string name)
        {
            var report = await _reportEngineTool.CreateReport(groupId, templateReportId, name);

            return new ReportModel { Name = name };
        }

        public async Task DeleteReport(Guid reportId)
        {
            await _reportService.DeleteReport(reportId);
        }

        public async Task<ReportModel> Deploy(string fileName, string groupId, string name)
        {
            var file = await _fileService.GetFile(fileName);
            var fileStream = file?.FileStream;
            if (fileStream == null)
            {
                throw new Exception($"File {fileName} is empty.");
            }

            var report = await _reportEngineTool.Deploy(fileStream, groupId, name);

            return new ReportModel { Name = name };
        }
    }
}
