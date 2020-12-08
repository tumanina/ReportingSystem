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

        public ReportManager(IReportService reportService, IFileService fileService)
        {
            _reportService = reportService;
            _fileService = fileService;
        }


        public async Task DeleteReport(string groupId, string reportId)
        {
            await _reportService.DeleteReport(groupId, reportId);
        }

        public async Task<ReportModel> Deploy(string fileName, string groupId, string name)
        {
            var file = await _fileService.GetFile(fileName);
            var fileStream = file.FileStream;
            if (fileStream == null)
            {
                throw new Exception($"File {fileName} is empty.");
            }

            return await _reportService.Deploy(fileStream, groupId, name);
        }
    }
}
