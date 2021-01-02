using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Managers
{
    public class ReportManager : IReportManager
    {
        private readonly IReportService _reportService;
        private readonly ITemplateService _templateService;
        private readonly IFileStorage _fileStorageService;
        private readonly IEnumerable<IReportEngineTool> _reportEngineTools;

        public ReportManager(IReportService reportService, ITemplateService templateService, IFileStorage fileStorage, IEnumerable<IReportEngineTool> reportEngineTools)
        {
            _reportService = reportService;
            _templateService = templateService;
            _fileStorageService = fileStorage;
            _reportEngineTools = reportEngineTools;
        }

        public async Task<ReportModel> CreateReport(ReportEngineToolEnum reportTool, string groupId, string templateReportId, string name)
        {
            var _reportEngineTool = _reportEngineTools.FirstOrDefault(t => t.ReportEngineTool == reportTool);
            if (_reportEngineTool == null)
            {
                throw new Exception($"No implementation for {reportTool}");
            }

            var report = await _reportEngineTool.CreateReport(groupId, templateReportId, name);

            return new ReportModel { Name = name };
        }

        public async Task DeleteReport(Guid reportId)
        {
            await _reportService.DeleteReport(reportId);
        }

        public async Task<ReportModel> Deploy(ReportEngineToolEnum reportTool, Guid reportId, Guid templateVersionId)
        {
            var report = await _reportService.GetReport(reportId);

            if (report == null)
            {
                throw new Exception($"Report with id {reportId} not found.");
            }

            var reportTemplate = await _templateService.GetTemplate(report.TemplateId);
            var teplateVersion = reportTemplate.Versions.FirstOrDefault(v => v.Id == templateVersionId);

            if (teplateVersion == null)
            {
                throw new Exception($"TemplateVersion with id {templateVersionId} not found.");
            }

            var _reportEngineTool = _reportEngineTools.FirstOrDefault(t => t.ReportEngineTool == reportTool);
            if (_reportEngineTool == null)
            {
                throw new Exception("No implementation for {reportTool}");
            }

            var file = await _fileStorageService.GetFile(teplateVersion.FileName);
            var fileStream = file?.FileStream;
            if (fileStream == null)
            {
                throw new Exception($"File {teplateVersion.FileName} is empty.");
            }

            var reportGroupId = report.Group?.ReportEngineToolGroups?.FirstOrDefault(r => r.ReportEngineTool == reportTool)?.GroupId;
            if (string.IsNullOrEmpty(reportGroupId))
            {
                throw new Exception($"Group {report.GroupId} does not have relations to groups on {reportTool}.");
            }

            var deployedReport = await _reportEngineTool.Deploy(fileStream, reportGroupId, report.Name);

            return new ReportModel 
            { 
                Id = report.Id, 
                Name = report.Name, 
                ReportEngineToolReports = new List<ReportEngineToolReportModel>
                { 
                    new ReportEngineToolReportModel
                    {
                        ReportEngineTool = reportTool,
                        ReportId = deployedReport.ReportId,
                        DatasetId = deployedReport.DatasetId,
                    }
                }
            };
        }
    }
}
