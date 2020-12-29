using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Managers
{
    internal class TemplateManager : ITemplateManager
    {
        private readonly ITemplateService _templateService;
        private readonly IFileStorage _fileStorage;

        public TemplateManager(ITemplateService templateService, IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
            _templateService = templateService;
        }

        public async Task<IEnumerable<TemplateModel>> GetTemplates()
        {
            return await _templateService.GetTemplates();
        }

        public async Task UploadTemplateFile(Guid templateId, Stream fileContent, string fileName, string version)
        {
            var template = await _templateService.GetTemplate(templateId);
            if (template == null)
            {
                throw new Exception($"Template {templateId} does not found.");
            }

            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithVersion = fileName.Replace(fileExtension, $"_{version.Trim()}{fileExtension}");

            await _fileStorage.UploadFile(fileNameWithVersion, fileContent);

            await _templateService.AddTemplateVersion(templateId, version, fileNameWithVersion);
        }
    }
}
